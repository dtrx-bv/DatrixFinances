(() => {
  const originalFetch = window.fetch;
  window.fetch = function(input, init = {}) {
    init.headers = init.headers || {};
    init.headers['Datrix-origin-company'] = '?';
    init.headers['Datrix-origin-platform'] = 'Swagger';
    init.headers['Datrix-origin-platform-signed-user'] = "?";

    return originalFetch(input, init);
  };
})();