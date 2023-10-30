using BroadcastMessages;

namespace BaseDefense.Messages.UpdateCurrencyMessages {

    public abstract class UpdateCurrencyMessage : Message {

        public readonly int Value;


        protected UpdateCurrencyMessage (int value) {
            Value = value;
        }

    }

}