window.stateManager = {
    save: function (key, str) {
        localStorage[key] = str;
    },
    load: function (key) {
        return localStorage[key];
    },
    remove: function (key) {
        if (localStorage.hasOwnProperty(key))
            delete localStorage[key];
    }
};
