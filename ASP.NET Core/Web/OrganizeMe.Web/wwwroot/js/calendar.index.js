function ShowDetailsNew(info) {
    var eventFromWeb = info["event"];
    $.ajax({
        url: '/Events/Details',
        datatype: "json",
        data: { id: eventFromWeb.id },
        type: "get",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        success: function (data) {
            $("#eventDetails").html(data);
        },
        error: function (xhr) {
            alert('error');
        }
    });

    $('#hoverDetails').show();
};

function DeleteButton() {
    var id = $('#deleteButton').data("id");
    $.ajax({
        url: '/Events/Delete',
        datatype: "json",
        data: { id: id },
        type: "get",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        success: function (data) {
            $("#eventDelete").html(data);
        },
        error: function (xhr) {
            alert('error');
        }
    })

    $('#hoverDelete').show();
};

document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
        header: {
            left: 'prev,next today ',
            right: 'dayGridMonth,timeGridWeek,timeGridDay',
            center: ' title',
        },
        titleFormat: {
            month: 'long',
            week: "long",
            day: 'numeric'
        },
        firstDay: 1,
        plugins: ['interaction', 'dayGrid', 'timeGrid', 'dayGrid'],
        defaultView: 'timeGridWeek',
        locale: 'en-gb',
        navLinks: true, // can click day/week names to navigate views
        editable: true,
        height: "auto",
        views: {
            dayGridMonth: { // name of view
                titleFormat: { year: 'numeric', month: '2-digit', day: '2-digit' }
                // other view-specific options here
            }
        },
        events: eventsJson,
        eventClick: ShowDetailsNew,
    });
    var ev = calendar.getEvents();
    for (var i = 0; i < ev.length; i++) {
        ev[i].color = 'red';
    }

    calendar.render();
});

$(window).on("load", function () {
    $('#deleteButton').click(function (e) { e.stopPropagation() });
    $('#hoverDetails').click(function () {
        $('#hoverDetails').hide();
    });
    $('#closeButtonDetails').click(function () {
        $('#hoverDetails').hide();
    });
});

$(window).on("load", function () {
    $('#hoverDelete').click(function () {
        $('#hoverDelete').hide();
    });
    $('#closeButtonDelete').click(function () {
        $('#hoverDelete').hide();
    });
    $('#cancelDelete').click(function () {
        $('#hoverDelete').hide();
    });
});