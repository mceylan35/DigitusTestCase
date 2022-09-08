namespace DigitusTestCase.WebAPP.Settings
{
    public class MongoDbConfig
    {
        public string Name { get; set; } = "DigitusDb";
        public string Host { get; set; }
        public int Port { get; set; }
        public string ConnectionString = "mongodb://digitustest:DhS6VqbRSUtUDH5R@ac-er8nwsw-shard-00-00.intfquw.mongodb.net:27017,ac-er8nwsw-shard-00-01.intfquw.mongodb.net:27017,ac-er8nwsw-shard-00-02.intfquw.mongodb.net:27017/?ssl=true&replicaSet=atlas-jgsyt5-shard-0&authSource=admin&retryWrites=true&w=majority";
    }
}
