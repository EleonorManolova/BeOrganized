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
        customButtons: {
        },
        events: [
            { // this object will be "parsed" into an Event Object
                id: 1,
                title: 'Home', // a property!
                start: '2020-03-16 10:30', // a property!
                end: '2020-03-16 13:30',// a property! ** see important note below about 'end' **
                color: 'darksalmon'
            }
        ],
    });
    var ev = calendar.getEvents();
    for (var i = 0; i < ev.length; i++) {
        ev[i].color = 'red';
    }

    calendar.render();
});
//$(document).ready(function () {
//    var ev = calendar.getEvents;
//    for (var i = 0; i < events.length; i++) {
//        ev[i].href = `/Events/Details/id=${ev[i].id}`
//    }
//    //var events = document.getElementsByClassName('fc-event');
//    //for (var i = 0; i < events.length; i++) {
//    //    events[i].href = `/Events/Details/id=${events[i].id}`
//    //}
//});
//remove scroll
    //$(document).ready(function () {
    //    var container = document.getElementsByClassName('fc-scroller');
    //    container[0].style.overflow = 'auto';
    //    container[0].style.height = '1180px'
    //});