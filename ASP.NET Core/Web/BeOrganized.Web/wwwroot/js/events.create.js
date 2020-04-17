function showCalendarLeft() {
    let calendarStart = document.getElementById("calendarStartDate");
    if (calendarStart.style.display === "inline-block") {
        calendarStart.style.display = "none";
    } else {
        calendarStart.style.display = "inline-block";
    }
}

//// Set time to local
//let dateTimeNow = new Date();
//var diference = dateTimeNow.getTimezoneOffset();
//dateTimeNow.setDate(dateTimeNow.getDate() + diference)
//$('#startTime').val = dateTimeNow.getTime();
//$('#endTime').val = dateTimeNow.setDate(dateTimeNow.getDate() + 30).getTime();

// Color selected CalendarColor
var selected = $("#color_me").css("background-color", function () {
    var color = $("option:selected", this).attr("class"); return color;
});
$("#color_me").change(function () {
    var color = $("option:selected", this).attr("class");
    $("#color_me").css("background-color", color);
});

//Google Map
function initAutocomplete() {
    var map;
    if (document.getElementById("map") === null) {
        if (document.getElementById('coordinates') === null || document.getElementById('coordinates').value === "") {
            map = new google.maps.Map(document.getElementById('mapWithCoordinates'), {
                center: { lat: 42.6979, lng: 23.3217 },
                zoom: 13,
                mapTypeId: 'roadmap'
            })
        }
        else {
            let coordinates = (document.getElementById('coordinates').value).toString().split(', ');
            let lat = +coordinates[0];
            let lng = +coordinates[1];

            map = new google.maps.Map(document.getElementById('mapWithCoordinates'), {
                center: { lat: lat, lng: lng },
                zoom: 18,
                mapTypeId: 'roadmap'
            })
        }
    }
    else {
        map = new google.maps.Map(document.getElementById('map'), {
            center: { lat: 42.6979, lng: 23.3217 },
            zoom: 13,
            mapTypeId: 'roadmap'
        })
    }

    // Create the search box and link it to the UI element.
    var input = document.getElementById('autocomplete4', {
        componentRestrictions: { country: 'bg' }
    });
    var searchBox = new google.maps.places.SearchBox(input);

    // Bias the SearchBox results towards current map's viewport.
    map.addListener('bounds_changed', function () {
        searchBox.setBounds(map.getBounds());
    });

    var markers = [];
    // Listen for the event fired when the user selects a prediction and retrieve
    // more details for that place.
    searchBox.addListener('places_changed', function () {
        var places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        // Clear out the old markers.
        markers.forEach(function (marker) {
            marker.setMap(null);
        });
        markers = [];

        // For each place, get the icon, name and location.
        var bounds = new google.maps.LatLngBounds();
        places.forEach(function (place) {
            if (!place.geometry) {
                console.log("Returned place contains no geometry");
                return;
            }
            var icon = {
                url: place.icon,
                size: new google.maps.Size(71, 71),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(17, 34),
                scaledSize: new google.maps.Size(25, 25)
            };

            // Create a marker for each place.
            markers.push(new google.maps.Marker({
                map: map,
                icon: icon,
                title: place.name,
                position: place.geometry.location
            }));

            if (place.geometry.viewport) {
                // Only geocodes have viewport.
                bounds.union(place.geometry.viewport);
            } else {
                bounds.extend(place.geometry.location);
            }
            document.getElementById('coordinates').value = place.geometry.location.toString();
        });
        map.fitBounds(bounds);

    });
};

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

    var diference = dateNow.getTimezoneOffset();
    dateNow.setDate(dateNow.getDate() + diference)
    console.log(dateNow);

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
            if (calendarStart.style.display === "inline-block") {
                $('#startDate').val(startDateOutput.toLocaleDateString("en-GB"));
            }
            $('#endDate').val(startDateOutput.toLocaleDateString("en-GB"));
        }
        );
    }
})(jQuery);