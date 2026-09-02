using System.Collections.Generic;
using System.Linq;

namespace Crypto_Trivia.Services
{
    public class Category
    {
        public string Name { get; set; }
        public List<Question> Questions { get; set; } = new();
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }
        public int Value { get; set; }
        public bool IsAnswered { get; set; }
    }

    public class GameService
    {
        private List<Category> _categories;
        private int _playerScore = 0;

        public GameService()
        {
            InitializeCategories();
        }

        private void InitializeCategories()
        {
            _categories = new List<Category>
            {
                new Category
                {
                    Name = "Bitcoin Basics",
                    Questions = new List<Question>
                    {
                        new Question { Id = 1, Text = "What year was Bitcoin created?", Answer = "2009", Value = 100 },
                        new Question { Id = 2, Text = "Who is Satoshi Nakamoto?", Answer = "Bitcoin's pseudonymous creator", Value = 200 },
                        new Question { Id = 3, Text = "What is Bitcoin's maximum supply?", Answer = "21 million", Value = 300 },
                        new Question { Id = 4, Text = "What is the smallest unit of Bitcoin called?", Answer = "Satoshi", Value = 400 },
                        new Question { Id = 5, Text = "What consensus mechanism does Bitcoin use?", Answer = "Proof of Work", Value = 500 }
                    }
                },
                new Category
                {
                    Name = "Ethereum Essentials",
                    Questions = new List<Question>
                    {
                        new Question { Id = 6, Text = "When did Ethereum launch?", Answer = "2015", Value = 100 },
                        new Question { Id = 7, Text = "What's the native token of Ethereum?", Answer = "Ether or ETH", Value = 200 },
                        new Question { Id = 8, Text = "What are smart contracts?", Answer = "Self-executing contracts with code on blockchain", Value = 300 },
                        new Question { Id = 9, Text = "What is an ERC-20 token?", Answer = "A fungible token standard on Ethereum", Value = 400 },
                        new Question { Id = 10, Text = "What is the Ethereum Virtual Machine (EVM)?", Answer = "A runtime for executing smart contracts", Value = 500 }
                    }
                },
                new Category
                {
                    Name = "Blockchain Technology",
                    Questions = new List<Question>
                    {
                        new Question { Id = 11, Text = "What is a blockchain?", Answer = "A distributed ledger of linked blocks", Value = 100 },
                        new Question { Id = 12, Text = "What does 'immutable' mean in blockchain?", Answer = "Data cannot be changed once recorded", Value = 200 },
                        new Question { Id = 13, Text = "What is a hash?", Answer = "A unique cryptographic fingerprint of data", Value = 300 },
                        new Question { Id = 14, Text = "What is a merkle tree?", Answer = "A tree of hashes used in blockchain", Value = 400 },
                        new Question { Id = 15, Text = "What is a consensus mechanism?", Answer = "A protocol for validating transactions", Value = 500 }
                    }
                },
                new Category
                {
                    Name = "Wallets & Transactions",
                    Questions = new List<Question>
                    {
                        new Question { Id = 16, Text = "What is a private key?", Answer = "A secret key that controls your crypto", Value = 100 },
                        new Question { Id = 17, Text = "What is a seed phrase?", Answer = "12-24 words used to recover wallets", Value = 200 },
                        new Question { Id = 18, Text = "What does 'HODL' mean in crypto?", Answer = "Hold On for Dear Life", Value = 300 },
                        new Question { Id = 19, Text = "What is gas in Ethereum?", Answer = "Fee paid for transaction processing", Value = 400 },
                        new Question { Id = 20, Text = "What is a hardware wallet?", Answer = "A physical device for storing crypto keys", Value = 500 }
                    }
                },
                new Category
                {
                    Name = "DeFi & Web3",
                    Questions = new List<Question>
                    {
                        new Question { Id = 21, Text = "What does DeFi stand for?", Answer = "Decentralized Finance", Value = 100 },
                        new Question { Id = 22, Text = "What is a liquidity pool?", Answer = "Pooled crypto assets enabling trades", Value = 200 },
                        new Question { Id = 23, Text = "What is Web3?", Answer = "Decentralized internet using blockchain", Value = 300 },
                        new Question { Id = 24, Text = "What is yield farming?", Answer = "Earning returns by providing liquidity", Value = 400 },
                        new Question { Id = 25, Text = "What is a DAO?", Answer = "Decentralized Autonomous Organization", Value = 500 }
                    }
                }
            };
        }

        public List<Category> GetCategories()
        {
            return _categories;
        }

        public Category GetCategory(string categoryName)
        {
            return _categories.FirstOrDefault(c => c.Name == categoryName);
        }

        public Question GetQuestion(string categoryName, int value)
        {
            var category = GetCategory(categoryName);
            return category?.Questions.FirstOrDefault(q => q.Value == value && !q.IsAnswered);
        }

        public void AnswerQuestion(string categoryName, int value)
        {
            var question = GetQuestion(categoryName, value);
            if (question != null)
            {
                question.IsAnswered = true;
                _playerScore += value;
            }
        }

        public int GetPlayerScore()
        {
            return _playerScore;
        }

        public void AddToScore(int amount)
        {
            _playerScore += amount;
        }

        public void ResetGame()
        {
            _playerScore = 0;
            InitializeCategories();
        }

        public bool IsGameOver()
        {
            return _categories.All(c => c.Questions.All(q => q.IsAnswered));
        }
    }
}
