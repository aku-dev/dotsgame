mergeInto(LibraryManager.library, { 

  JSShowYandexAds: function () {
	ysdk.adv.showFullscreenAdv({ callbacks: {
				onClose: function(wasShown) { unityInstance.SendMessage('GameManager', 'GameUnPause'); },
				onOpen: function() { unityInstance.SendMessage('GameManager', 'GamePause'); }
	}}); 	
  },
  
  JSShowYandexRewarded: function () {
	ysdk.adv.showRewardedVideo({ callbacks: {
				onRewarded: function() { unityInstance.SendMessage('Ads_Manager', 'RewardedSuccessful'); },
				onError: function(e) {  },
				onClose: function() { unityInstance.SendMessage('Ads_Manager', 'RewardedClose'); },
				onOpen: function() {  }
	}}); 	
  }
 
});