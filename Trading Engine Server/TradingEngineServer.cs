using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Logging;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Core
{
    sealed class TradingEngineServer : BackgroundService, iTradingEngineServer
    {
        private readonly TradingEngineServerConfiguration _tradingEngineServerConfig;
        private readonly ITextLogger _logger;

        public TradingEngineServer(ITextLogger textLogger, IOptions<TradingEngineServerConfiguration> config)
        {
            // Ensure logger and configuration are not null
            _logger = textLogger ?? throw new ArgumentNullException(nameof(textLogger));
            _tradingEngineServerConfig = config?.Value ?? throw new ArgumentNullException(nameof(config));
        }

        // Allows manual running of the server
        public Task Run(CancellationToken token) => ExecuteAsync(token);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Available Commands:");
            Console.WriteLine("ADD_ORDER B|S [price] [quantity] [username] [securityId] - Adds a new order. B = Buy, S = Sell.");
            Console.WriteLine("MODIFY_ORDER [orderId] [username] [securityId] [newPrice] [newQuantity] - Modifies an existing order.");
            Console.WriteLine("CANCEL_ORDER [orderId] [username] [securityId] - Cancels an existing order.");
            Console.WriteLine("MATCH [securityId] - Matches orders for a specific security.");
            Console.WriteLine("DISPLAY [securityId] - Displays the order book for a specific security.");
            Console.WriteLine("EXIT - Shuts down the trading engine.");
            Console.WriteLine("HELP - Displays this list of commands.");
            // Log server startup
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "TradingEngineMainThread";
            }

            _logger.Info(nameof(TradingEngineServer), "Starting Trading Engine");

            // Command queue for processing incoming commands
            var commandQueue = new BlockingCollection<string>();

            // Start a separate task to read and enqueue commands (e.g., from CLI)
            _ = Task.Run(() => ReadCommands(commandQueue, stoppingToken), stoppingToken);

            // Create a dictionary of order books, one for each security
            var orderbooks = new Dictionary<int, MatchingOrderBook>();
            foreach (var security in InstrumentRepository.Instruments)
            {
                orderbooks[security.SecurityId] = new MatchingOrderBook(security);
            }

            // Main processing loop
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Wait for and process the next command
                    if (commandQueue.TryTake(out var command, TimeSpan.FromMilliseconds(100)))
                    {
                        ProcessCommand(command, orderbooks);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Graceful shutdown when stoppingToken is canceled
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error(nameof(TradingEngineServer), ex);
                }
            }

            // Log server shutdown
            _logger.Info(nameof(TradingEngineServer), "Stopped Trading Engine");
        }

        // Reads commands (e.g., from CLI or file) and adds them to the queue
        private void ReadCommands(BlockingCollection<string> commandQueue, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var command = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(command))
                {
                    commandQueue.Add(command);
                }
            }
        }

        // Parses and processes commands, routing them to the appropriate order book
        private void ProcessCommand(string command, Dictionary<int, MatchingOrderBook> orderbooks)
        {
            var parts = command.Split(' ');
            var action = parts[0].ToUpperInvariant();

            try
            {
                switch (action)
                {
                    case "ADD_ORDER":
                        // Example: ADD_ORDER B 100 10 User1 1
                        var side = parts[1] == "B";
                        var price = long.Parse(parts[2]);
                        var quantity = uint.Parse(parts[3]);
                        var username = parts[4];
                        var securityId = int.Parse(parts[5]);

                        if (orderbooks.TryGetValue(securityId, out var addOrderbook))
                        {
                            var orderIdforAdd = Order.GenerateUniqueOrderId();
                            var newOrder = new Order(new OrderCore(orderIdforAdd, username, securityId), price, quantity, side);
                            addOrderbook.AddOrder(newOrder);
                            _logger.Info(nameof(TradingEngineServer), $"Order added to Security {securityId}: {newOrder.ToString}");
                            Console.WriteLine($"Order Added: [OrderId: {orderIdforAdd}, Side: {(side ? "Buy" : "Sell")}, Price: {price}, Quantity: {quantity}, SecurityId: {securityId}, Username: {username}]");
                        }
                        else
                        {
                            Console.WriteLine($"Invalid SecurityId: {securityId}");
                        }
                        break;

                    case "MATCH":
                        // Example: MATCH 1
                        securityId = int.Parse(parts[1]);
                        if (orderbooks.TryGetValue(securityId, out var matchOrderbook))
                        {
                            var matchResult = matchOrderbook.Match();
                            Console.WriteLine($"Match result for Security {securityId}:\n{matchResult}");
                            _logger.Info(nameof(TradingEngineServer), $"Matching completed for Security {securityId}");
                        }
                        else
                        {
                            Console.WriteLine($"Invalid SecurityId: {securityId}");
                        }
                        break;

                    case "MODIFY_ORDER":
                        // Example: MODIFY_ORDER 1 User1 1 120 50
                        var orderId = long.Parse(parts[1]);
                        username = parts[2];
                        securityId = int.Parse(parts[3]);
                        var newPrice = long.Parse(parts[4]);
                        var newQuantity = uint.Parse(parts[5]);

                        if (orderbooks.TryGetValue(securityId, out var modifyOrderbook))
                        {
                            // Find the existing order by ID
                            var existingOrder = modifyOrderbook.GetBidOrders().FirstOrDefault(o => o.CurrentOrder.OrderId == orderId)
                                                ?? modifyOrderbook.GetAskOrders().FirstOrDefault(o => o.CurrentOrder.OrderId == orderId);

                            if (existingOrder != null)
                            {
                                if (existingOrder.CurrentOrder.Username == username)
                                {
                                    // Remove the existing order
                                    var cancelOrder = new CancelOrder(new OrderCore(orderId, username, securityId));
                                    modifyOrderbook.RemoveOrder(cancelOrder);

                                    // Add the modified order
                                    var modifiedOrder = new Order(new OrderCore(orderId, username, securityId), newPrice, newQuantity, existingOrder.CurrentOrder.IsBuySide);
                                    modifyOrderbook.AddOrder(modifiedOrder);

                                    _logger.Info(nameof(TradingEngineServer), $"Order {orderId} modified for Security {securityId} by {username}: New Price {newPrice}, New Quantity {newQuantity}");
                                    Console.WriteLine($"Order {orderId} modified successfully.");
                                }
                                else
                                {
                                    Console.WriteLine($"Username mismatch: Provided {username}, but order belongs to {existingOrder.CurrentOrder.Username}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"OrderId {orderId} not found in Security {securityId}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Invalid SecurityId: {securityId}");
                        }
                        break;

                    case "CANCEL_ORDER":
                        // Example: CANCEL_ORDER 1 User1 1
                        orderId = long.Parse(parts[1]);
                        username = parts[2];
                        securityId = int.Parse(parts[3]);

                        if (orderbooks.TryGetValue(securityId, out var cancelOrderbook))
                        {
                            // Find the order in the orderbook by ID
                            var existingOrder = cancelOrderbook.GetBidOrders().FirstOrDefault(o => o.CurrentOrder.OrderId == orderId)
                                                ?? cancelOrderbook.GetAskOrders().FirstOrDefault(o => o.CurrentOrder.OrderId == orderId);

                            if (existingOrder != null)
                            {
                                if (existingOrder.CurrentOrder.Username == username)
                                {
                                    var cancelOrder = new CancelOrder(new OrderCore(orderId, username, securityId));
                                    cancelOrderbook.RemoveOrder(cancelOrder);
                                    _logger.Info(nameof(TradingEngineServer), $"Order {orderId} canceled for Security {securityId} by {username}");
                                    Console.WriteLine($"Order {orderId} cancelled successfully.");
                                }
                                else
                                {
                                    Console.WriteLine($"Username mismatch: Provided {username}, but order belongs to {existingOrder.CurrentOrder.Username}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"OrderId {orderId} not found in Security {securityId}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Invalid SecurityId: {securityId}");
                        }
                        break;

                    case "DISPLAY":
                        // Example: DISPLAY 1
                        securityId = int.Parse(parts[1]);

                        if (orderbooks.TryGetValue(securityId, out var displayOrderbook))
                        {
                            Console.WriteLine($"Order Book for Security {securityId}:");
                            Console.WriteLine("Bids:");
                            foreach (var bid in displayOrderbook.GetBidOrders())
                            {
                                Console.WriteLine(bid.CurrentOrder);
                            }
                            Console.WriteLine("Asks:");
                            foreach (var ask in displayOrderbook.GetAskOrders())
                            {
                                Console.WriteLine(ask.CurrentOrder);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Invalid SecurityId: {securityId}");
                        }
                        break;

                    case "HELP":
                        Console.WriteLine("Available Commands:");
                        Console.WriteLine("ADD_ORDER B|S [price] [quantity] [username] [securityId] - Adds a new order. B = Buy, S = Sell.");
                        Console.WriteLine("MODIFY_ORDER [orderId] [username] [securityId] [newPrice] [newQuantity] - Modifies an existing order.");
                        Console.WriteLine("CANCEL_ORDER [orderId] [username] [securityId] - Cancels an existing order.");
                        Console.WriteLine("MATCH [securityId] - Matches orders for a specific security.");
                        Console.WriteLine("DISPLAY [securityId] - Displays the order book for a specific security.");
                        Console.WriteLine("EXIT - Shuts down the trading engine.");
                        Console.WriteLine("HELP - Displays this list of commands.");
                        break;

                    case "EXIT":
                        _logger.Info(nameof(TradingEngineServer), "Shutdown command received.");
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Unknown command. Type HELP for a list of commands.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(TradingEngineServer), $"Error processing command: {command}. {ex.Message}");
            }
        }
    }
}
