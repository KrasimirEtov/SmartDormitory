$(() => {
    "use strict";

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .build();

    connection.on("ReceiveNotification", function (message) {
        //console.log(message)
        // debugger
        let count = document.getElementById('unseen').innerHTML;
        document.getElementById('unseen').innerHTML = `${++count}`;
    });

    connection.start().catch(function (err) {
        return console.error(err.toString());
    });
});