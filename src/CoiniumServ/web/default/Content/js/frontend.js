$(function () {
    // process the search boxes
    $('#block-search').submit(function () {
        var height = $("#block-height").val();
        $("#block-search").attr("action", $(location).attr('pathname') + 'block/' + height);
    });
    $('#address-search').submit(function () {
        var address = $("#address").val();
        $("#address-search").attr("action", $(location).attr('pathname') + 'account/address/' + address);
    });

    $('time.timeago').timeago(); // parse date & time's as time-ago texts.
});
