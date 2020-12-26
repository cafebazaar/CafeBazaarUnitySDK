namespace CafeBazaar.Games.BasicApi
{
    public struct BazaarGamesClientConfiguration
    {
        private readonly bool mEnableSavedGames;

        public static readonly BazaarGamesClientConfiguration DefaultConfiguration = new Builder().Build();
        
        
        private BazaarGamesClientConfiguration(Builder builder)
        {
            this.mEnableSavedGames = builder.HasEnableSaveGames();
        }
        
        
        public class Builder
        {
            /// <summary>
            /// The flag to enable save games. Default is false.
            /// </summary>
            private bool mEnableSaveGames = false;

            public BazaarGamesClientConfiguration Build()
            {
                return new BazaarGamesClientConfiguration(this);
            }

            /// <summary>
            /// Enables the saved games.
            /// </summary>
            /// <returns>The builder.</returns>
            public Builder EnableSavedGames()
            {
                mEnableSaveGames = true;
                return this;
            }

            /// <summary>
            /// Determines whether this instance has enable save games.
            /// </summary>
            /// <returns><c>true</c> if this instance has enable save games; otherwise, <c>false</c>.</returns>
            internal bool HasEnableSaveGames()
            {
                return mEnableSaveGames;
            }
        }

        /// </summary>
        /// <value><c>true</c> if enable saved games; otherwise, <c>false</c>.</value>
        public bool EnableSavedGames
        {
            get { return mEnableSavedGames; }
        }

        public static bool operator ==(BazaarGamesClientConfiguration c1, BazaarGamesClientConfiguration c2)
        {
            if (c1.EnableSavedGames != c2.EnableSavedGames )
            {
                return false;
            }

            return true;
        }

        public static bool operator !=(BazaarGamesClientConfiguration c1, BazaarGamesClientConfiguration c2)
        {
            return !(c1 == c2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + EnableSavedGames.GetHashCode(); 
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return this == (BazaarGamesClientConfiguration)obj;
        }
    }
}
