var silentGoToUrl = function (page, title, url, replace) {
    if ("undefined" !== typeof history.pushState) {
        if (title === undefined || title === null || title === '') title = document.title;
        if (replace !== undefined && !!replace) {
            history.replaceState({ page: page }, title, url);
        } else {
            history.pushState({ page: page }, title, url);
        }
    } else {
        window.location.assign(url);
    }
}

export { silentGoToUrl };