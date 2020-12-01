mergeInto(LibraryManager.library, {
   OpenURLNewTab: function(url) {
       console.log("opening " + Pointer_stringify(url));
       var win = window.open(Pointer_stringify(url), '_blank');
       win.focus();
   },

   FullScreen: function() {
       document.querySelector("#unity-canvas").requestFullscreen();
   },
});