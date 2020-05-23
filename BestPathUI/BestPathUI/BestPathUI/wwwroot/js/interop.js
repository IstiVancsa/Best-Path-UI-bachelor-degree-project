var map, cityAutoComplete, mapCounter = 0;
var city_initialized = false;
var marker;
var directionsService;
var directionsRenderer;

function createMap() {
    google.maps.event.addDomListener(window, 'load', initializeMap);
}

function createLocationAutocomplete() {
    google.maps.event.addDomListener(window, 'load', initializeLocationAutocomplete);
}

function initializeMap() {

    if (mapCounter > 0) {
        var mapOptions = {
            center: new google.maps.LatLng(47.151726, 27.587914),
            zoom: 15
        };
        map = new google.maps.Map(document.getElementById('map_canvas'), mapOptions);
    }
    mapCounter++;
}

function showRoute(origin, destination, wayPoints) {
    directionsService = new google.maps.DirectionsService();
    directionsRenderer = new google.maps.DirectionsRenderer({
        draggable: true,
        map: map
    });

    var javascriptWayPoints = [];
    for (var i = 0; i < wayPoints.length; i++) {
        javascriptWayPoints.push({
            location: new google.maps.LatLng(wayPoints[i].lat, wayPoints[i].lng)
        })
    }

    directionsService.route({
        origin: new google.maps.LatLng(origin.lat, origin.lng),
        destination: new google.maps.LatLng(destination.lat, destination.lng),
        waypoints: javascriptWayPoints,
        optimizeWaypoints: false,
        travelMode: 'DRIVING'
    }, function (response, status) {
        if (status == "OK") {
            directionsRenderer.setDirections(response);
        } else {
            window.alert('Directions request failed due to ' + status);
        }
    })
}

function initializeAutocompletes() {

    var options = {
        types: ['(cities)']
    }

    var input = document.getElementById('city_search');
    cityAutoComplete = new google.maps.places.Autocomplete(input, options);

    google.maps.event.addListener(cityAutoComplete, 'place_changed', function () {
        var place = cityAutoComplete.getPlace();

        var current_lat = parseFloat(place.geometry.location.lat(), 10);
        var current_long = parseFloat(place.geometry.location.lng(), 10);

        var lat_hidden = document.getElementById('lat_hidden');
        var lng_hidden = document.getElementById('lng_hidden');

        lat_hidden.value = current_lat;
        lng_hidden.value = current_long;

        var location = {
            lat: current_lat,
            lng: current_long
        };

        DotNet.invokeMethodAsync('BestPathUI', 'SetLocation', location);

        input.value = place.name;
        city_initialized = true;
    });
}

function enableTextbox(chkId, txtId) {
    if (document.getElementById(chkId).checked) {
        if (city_initialized == true && document.getElementById('city_search').value != '')
            document.getElementById(txtId).disabled = false;
    }
    else
        document.getElementById(txtId).disabled = true;
}

function showLocation(location) {
    marker = new google.maps.Marker({
        position: new google.maps.LatLng(location.lat, location.lng),
        map: map
    });
}

function hideLocation() {
    marker.setMap(null);
}

function removeDirections() {
    if (typeof marker != "undefined")
        marker.setMap(null);
    if (typeof directionsRenderer != "undefined")
        directionsRenderer.setMap(null);
}