// SignalR
let connection = null;

setupConnection = () => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/eventshub")
        .build();

    connection.start()
        .catch(err => console.error(err.toString()));
};

setupConnection();

function EventChange(info) {
    const id = info.event.id;
    const startDateTime = info.event.start.toISOString();
    const endDateTime = info.event.end.toISOString();
    if (info.event.extendedProps.iscompleted != null) {
        connection.invoke("HabitsChange", { id, startDateTime, endDateTime })
    }
    else {
        connection.invoke("EventsChange", { id, startDateTime, endDateTime })
    }
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
            alert("An error has occured! Please try again later or contact the support.");
        }
    });
}



function ShowDetails(info) {
    var elementFromWeb = info["event"];
    if (info.event.extendedProps.iscompleted != null) {
        // Is Habit
        callAjax(elementFromWeb.id, "/Habits/Details", "#elementDetails")
    }
    else {
        // Is Event
        callAjax(elementFromWeb.id, "/Events/Details", "#elementDetails")
    }
    $('#hoverDetails').show();

};

function DeleteButton() {
    let id = $('#deleteButton').data("id");
    callAjax(id, '/Events/Delete', "#elementDelete");
    $('#hoverDelete').show();
};

function DeleteButtonHabit() {
    let id = $('#deleteButton').data("count");
    callAjax(id, '/Habits/Delete', "#elementDelete");
    $('#hoverDelete').show();
};

function ShowDetailsResult() {
    let id = $('#detailsButton').data("id");
    callAjax(id, '/Events/Details', "#elementDetails");
    $('#hoverDetails').show();
};

document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    let all = eventsJson.concat(habitsJson);
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
        plugins: ['interaction', 'dayGrid', 'timeGrid', 'dayGrid', 'googleCalendar'],
        defaultView: 'timeGridWeek',
        locale: 'en-gb',
        navLinks: true, // can click day/week names to navigate views
        editable: true,
        allDaySlot: false,
        nowIndicator: true,
        height: 720,
        //height: 'parent',
        views: {
            dayGridMonth: { // name of view
                titleFormat: { year: 'numeric', month: '2-digit', day: '2-digit' }
                // other view-specific options here
            }
        },
        googleCalendarApiKey: apiKey,
        eventSources: [
            {
                //  all
            },
            {
                googleCalendarId: '',
                editable: false
            }
        ],
        //events: 'eleonor.manolova@gmail.com',
        events: all,
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