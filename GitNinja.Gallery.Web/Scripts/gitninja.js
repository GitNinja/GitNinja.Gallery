(function () {
    $('.nav-notifications').popover({ html: true, content: $('div.notification-container').html() });
    $('.notification-target').html($('div.notification-container').html());
})();