using System.Collections.Generic;

namespace BaseDefense.BroadcastMessages.Messages {

    public class UnlockedGunsMessage : Message {

        public readonly List<int> unlockedGuns;
        
        
        public UnlockedGunsMessage (List<int> unlockedGuns) => this.unlockedGuns = unlockedGuns;

    }

}