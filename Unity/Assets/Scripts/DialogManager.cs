    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public partial class DialogManager
    {
        [SerializeField] private RuntimeAnimatorController avatar;

        public async UniTask WelcomeDialog()
        {
            await ShowMessage(avatar, "Hey your first day at the job, isn't it?");
            await ClickToContinue();
            await ShowMessage(avatar, "I'm Ricky, your new partner. I'll be helping you out with the deliveries.");
            await ClickToContinue();
            await ShowMessage(avatar, "We have a lot of parcels to deliver today so let's get started!");
            await ClickToContinue();
            await ShowMessage(avatar, "Time is money so we have to be quick. I'll be at the back getting the right parcel.");
            await ClickToContinue();
            await ShowMessage(avatar, "When we are at our destination just give me a sign and \n... you know ...");
            await ClickToContinue();
            await ShowMessage(avatar, "I'll give it a shove!");
            await ClickToContinue();
            await ShowMessage(avatar, "We aren't delivering anything fragile, are we?");
            await ClickToContinue();
            await Hide();
        }
        
        private bool parcelCooldownDialogTriggered;

        public async UniTask ParcelCooldownDialog()
        {
            if (parcelCooldownDialogTriggered)
                return;
            parcelCooldownDialogTriggered = true;
            await ShowMessage(avatar, "I can't throw out parcels so fast!");
            await ClickToContinue();
            await ShowMessage(avatar, "You have to wait a few seconds before I can throw out another one.");
            await ClickToContinue();
            await ShowMessage(avatar, "You can always check the cooldown at the top right corner of the screen.");
            await ClickToContinue();
            await Hide();
        }
        
        private bool fasterTruckDialogTriggered;

        public async UniTask FasterTruckDialog()
        {
            if (fasterTruckDialogTriggered)
                return;
            fasterTruckDialogTriggered = true;
            await ShowMessage(avatar, "Hey, notice any difference?");
            await ClickToContinue();
            await ShowMessage(avatar, "I've got a little more juice out of the engine. We should be able to save even more time now.");
            await ClickToContinue();
            await Hide();
        }

        public async UniTask LongTimeDialog()
        {
            await ShowMessage(avatar, "Oh, my tea is done! I'll be right back.");
            await ClickToContinue();
            await ShowMessage(avatar, "Can't imagine it has been 5 minutes already. Time flies when you are having fun!");
            await ClickToContinue();
            await Hide();
        }

        public async UniTask ManyParcelsDialog()
        {
            await ShowMessage(avatar, "98 ... 99 ... 100!");
            await ClickToContinue();
            await ShowMessage(avatar, "This must be a new record! Are you sure we didn't lose any along the way?");
            await ClickToContinue();
            await Hide();
        }
        
        public async UniTask EndOfGameDialog()
        {
            await ShowMessage(avatar, "Congratulations! You have reached the end of the game.");
            await ClickToContinue();
            await ShowMessage(avatar, "Thank you for playing! I would love to hear your feedback and a rating would be very much appreciated.");
            await ClickToContinue();
            await ShowMessage(avatar, "If you want to play any of the levels again you can navigate through them by pressing the page up and down keys.");
            await ClickToContinue();
            await ShowMessage(avatar, "I hope you enjoyed your time here. Have a great day!");
            await ClickToContinue();
            await Hide();
        }
    }