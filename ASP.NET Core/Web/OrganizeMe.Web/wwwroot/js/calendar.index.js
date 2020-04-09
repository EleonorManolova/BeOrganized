function EventChange(info) {
    const id = info.event.id;
    const startDateTime = info.event.start.toISOString();
    const endDateTime = info.event.end.toISOString();

    connection.invoke("EventsChange", { id, startDateTime, endDateTime })
}

function callAjax(eventId, url, divId) {
    $.ajax({
        url: url,
        datatype: "json",
        data: { id: eventId },
        type: "get",
        contenttype: 'application/json; charset=utf-8',
        async: true,
        success: function (data) {
            $(divId).html(data);
        },
        error: function (xhr) {
            alert('error');
        }
    });
}

function ShowDetails(info) {
    var eventFromWeb = info["event"];
    callAjax(eventFromWeb.id, "/Events/Details", "#eventDetails")
    $('#hoverDetails').show();
};

function DeleteButton() {
    let id = $('#deleteButton').data("id");
    callAjax(id, '/Events/Delete', "#eventDelete");
    $('#hoverDelete').show();
};

function ShowDetailsResult() {
    let id = $('#detailsButton').data("id");
    callAjax(id, '/Events/Details', "#eventDetails");
    $('#hoverDetails').show();
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
        eventClick: ShowDetails,
        eventDrop: EventChange,
        eventResize: EventChange,
    });
    var ev = calendar.getEvents();
    for (var i = 0; i < ev.length; i++) {
        ev[i].color = 'red';
    }

    calendar.render();
});

$(window).on("load", function () {
    //$('#deleteButton').click(function (e) { e.stopPropagation() });
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