function showCalendarLeft() {
    let calendarStart = document.getElementById("calendarStartDate");
    if (calendarStart.style.display === "inline-block") {
        calendarStart.style.display = "none";
    } else {
        calendarStart.style.display = "inline-block";
    }
}
function showCalendarRight() {
    let calendarStart = document.getElementById("calendarStartDate");
    if (calendarStart.style.display === "block") {
        calendarStart.style.display = "none";
    } else {
        calendarStart.style.display = "block";
    }
}
(function ($) {
    let dateNow = new Date();
    let themonth = dateNow.getMonth(); // get this month
    let theyear = dateNow.getFullYear(); // get this year
    renderCal(themonth, theyear);


    $('.minusmonth').click(function () {
        themonth += -1;
        if (themonth <= -1) {
            themonth = 11;
            theyear--;
        }
        renderCal(themonth, theyear);
    });

    $('.addmonth').click(function () {
        themonth += 1;
        if (themonth >= 12) {
            themonth = 0;
            theyear++;
        }
        renderCal(themonth, theyear);
    });

    function renderCal(themonth, theyear) {
        $('.calendar li').remove();
        $('.calendar ul').
            end('<li>Mon</li><li>Tu</li><li>We</li><li>Th</li><li>Fr</li><li>Sa</li><li>Su</li>');

        days = numDays(themonth, dateNow.getYear()); // get number of days in the month
        fDay = firstDay(themonth, theyear) - 2; // find what day of the week the 1st lands on
        months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']; // month names

        $('.calendar p.monthname').text(months[themonth] + ' ' + (theyear).toString()); // add month name to calendar
        if (fDay < -1) {
            fDay += 7;
        }
        for (var i = 0; i <= fDay; i++) { // place the first day of the month in the correct position
            $('<li>&nbsp;</li>').appendTo('.calendar ul');
        }

        for (var i = 1; i <= days; i++) { // write out the days
            $('<li>' + i + '</li>').appendTo('.calendar ul');
        }

        function firstDay(month, year) {
            return new Date(year, month, 1).getDay();
        }

        function numDays(month, year) {
            return new Date(year, month + 1, 0).getDate();
        }

        $('.calendar li').click(function () { // toggle selected date
            $('.calendar li').removeClass('red');
            $(this).addClass('red');
            let calendarStart = document.getElementById("calendarStartDate");
            let startDateOutput = new Date(`${months[themonth]} ${$(this).text()}, ${(theyear).toString()} 00:00:00`);
            if (calendarStart.classList.contains('leftPosition')) {
                $('#startDate').val(startDateOutput.toLocaleDateString("en-GB"));
            }
            $('#endDate').val(startDateOutput.toLocaleDateString("en-GB"));
        }
        );
    }

    //GoogleMaps PlaceSearch
    var ac = new google.maps.places.Autocomplete(document.getElementById('autocomplete4'), {
        componentRestrictions: { country: 'bg' }
    });
    google.maps.event.addListener(ac, 'place_changed', function () {
        var place = ac.getPlace;
        console.log(place.formatted_address);
        console.log(place.url);
     //   console.log(place.geometry.location);
    })
})(jQuery);