using System.Collections.Generic;
using BroadcastMessages;

namespace BaseDefense.Messages {

    public class UnlockedGunsMessage : Message {

        public readonly List<int> unlockedGuns;
        
        
        public UnlockedGunsMessage (List<int> unlockedGuns) => this.unlockedGuns = unlockedGuns;

    }

}