mergeInto(LibraryManager.library, {
  RedirectToURL: function (p_url) {
    var s_url = Pointer_stringify(p_url)
    window.location.href = s_url;
  }
})
