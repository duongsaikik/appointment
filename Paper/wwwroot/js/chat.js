"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("sendButton").disabled = true;

connection.on("send", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMess", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

//Disable the send button until connection is established.

var token = $('input[type=hidden][name=__RequestVerificationToken]', document).
    val();
$.ajaxSetup({
    // Disable caching of AJAX responses
    cache: false,
    headers: {
        'RequestVerificationToken': token, 'ServiceRequestId':
            $('#ServiceRequest_RowKey').val()
    }
});

$.get('/ServiceRequests/ServiceRequest/ServiceRequestMessages?serviceRequestId='
    + $('#ServiceRequest_RowKey').val(),
    function (data, status) {
        addMessagesToList(data);
    });


function addMessagesToList(messages) {
    if (messages.length === 0) {
        $('.noMessages').removeClass('hide');
    }
    $.each(messages, function (index) {
        var message = messages[index];
        displayMessage(message);
    });
   
};
function addMessage(message) {
    if (message.PartitionKey !== $('#ServiceRequest_RowKey').val()) {
        return;
    }
    if (message !== null) {
        $('.noMessages').addClass('hide');
    }
    displayMessage(message);
   
};

function displayMessage(message) {
   
    $('#messagesList').append(
        '<li class="list-group-item d-flex justify-content-between align-items-start">' +
        ' <div class="ms-2 me-auto">' +
        '  <div class="fw-bold">' + message.FromDisplayName + '</div>'
        + message.Message +
        '</div>' +
        '<span class="badge bg-primary rounded-pill">' + (new Date(message.MessageDate)).toLocaleString() + '</span>' +
        '</li>'
    );
};
  //'<li class="list-group-item ' + isCustomer + ' white-text padding-10px">' +
        //'<div class="col s12 padding-0px">' +
        //'<div class="col s8 padding-0px"><b>' + message.FromDisplayName + '</b ></div > ' +
        //'<div class="col s4 padding-0px font-size-12px right-align">' + (new
        //    Date(message.MessageDate)).toLocaleString() + '</div>' +
        //'</div><br>' + message.Message + '</li>'

//connection.on("send", function () {
//    var token = $('input[type=hidden][name=__RequestVerificationToken]', document).
//        val();
//    $.ajaxSetup({
//        // Disable caching of AJAX responses
//        cache: false,
//        headers: {
//            'RequestVerificationToken': token, 'ServiceRequestId':
//                $('#ServiceRequest_RowKey').val()
//        }
//    });

//    $.get('/ServiceRequests/ServiceRequest/ServiceRequestMessages?serviceRequestId='
//        + $('#ServiceRequest_RowKey').val(),
//        function (data, status) {
//            addMessagesToList(data);
//        });


//    function addMessagesToList(messages) {
//        if (messages.length === 0) {
//            $('.noMessages').removeClass('hide');
//        }
//        $.each(messages, function (index) {
//            var message = messages[index];
//            displayMessage(message);
//        });
//        scrollToLatestMessages();
//    };
//    function addMessage(message) {
//        if (message.PartitionKey !== $('#ServiceRequest_RowKey').val()) {
//            return;
//        }
//        if (message !== null) {
//            $('.noMessages').addClass('hide');
//        }
//        displayMessage(message);
//        scrollToLatestMessages();
//    };

//    function displayMessage(message) {
//        var isCustomer = $("#hdnCustomerEmail").val() === message.FromEmail ? 'blue lighten - 1' : 'teal lighten - 2';
//        $('#messagesList').append(
//            '<li class="list-group-item d-flex justify-content-between align-items-start">' +
//            ' <div class="ms-2 me-auto">' +
//            '  <div class="fw-bold">' + message.FromDisplayName + '</div>'
//            + message.Message +
//            '</div>' +
//            '<span class="badge bg-primary rounded-pill">' + (new Date(message.MessageDate)).toLocaleString() + '</span>' +
//            '</li>'
//        );
//    };
//});

//connection.start().then(function () {
//    document.getElementById("sendButton").disabled = false;
//}).catch(function (err) {
//    return console.error(err.toString());
//});





$('#txtMessage').keypress(function (e) {
    var key = e.which;
    if (key == 13) {
       
        var message = new Object();
        message.Message = $('#txtMessage').val();
        message.PartitionKey = $('#ServiceRequest_RowKey').val();
    
        $.post('/ServiceRequests/ServiceRequest/CreateServiceRequestMessage',
            { message: message },
            function (data, status, xhr) {
                if (data) {
                    $('.noMessages').addClass('hide');
                    $('#txtMessage').val('');
                }
            });
        
    }
});
