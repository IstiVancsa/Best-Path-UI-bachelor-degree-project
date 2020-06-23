var map, cityAutoComplete, mapCounter = 0;
var city_initialized = false;
var marker;
var directionsService;
var directionsRenderer;
var lastObjRef;
var lastOrigin;
var lastDestination;

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

function showRoute(origin, destination, wayPoints, optimize) {
    //console.log(origin);
    //console.log(destination);
    //console.log(wayPoints);
    //console.log(optimize);
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
        optimizeWaypoints: optimize,
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

        input.value = place.formatted_address;
        city_initialized = true;
    });
}

function getDistance(origin, destination) {
    lastOrigin = origin;
    lastDestination = destination;
    var distanceServiceWithHighWays = new google.maps.DistanceMatrixService();
    var distanceServiceWithOutHighWays = new google.maps.DistanceMatrixService();
    distanceServiceWithHighWays.getDistanceMatrix({
        origins: [origin],
        destinations: [destination],
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.METRIC,
        avoidHighways: false,
        avoidTolls: false
    }, withHighWaysCallback);

    distanceServiceWithOutHighWays.getDistanceMatrix({
        origins: [origin],
        destinations: [destination],
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.METRIC,
        avoidHighways: true,
        avoidTolls: false
    }, withOutHighWaysCallback);
}

function withHighWaysCallback(response, status) {
    DotNet.invokeMethodAsync('BestPathUI', 'SetResult', response.rows[0].elements[0], lastOrigin, lastDestination);
}

function withOutHighWaysCallback(response, status) {
    DotNet.invokeMethodAsync('BestPathUI', 'SetResult', response.rows[0].elements[0], lastOrigin, lastDestination);
}

function getDistanceObjRef(objref,origin, destination) {
    lastObjRef = objref;
    var distanceServiceWithHighWays = new google.maps.DistanceMatrixService();
    var distanceServiceWithOutHighWays = new google.maps.DistanceMatrixService();
    distanceServiceWithHighWays.getDistanceMatrix({
        origins: [origin],
        destinations: [destination],
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.METRIC,
        avoidHighways: false,
        avoidTolls: false
    }, withHighWaysCallbackObjRef);

    distanceServiceWithOutHighWays.getDistanceMatrix({
        origins: [origin],
        destinations: [destination],
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.METRIC,
        avoidHighways: true,
        avoidTolls: false
    }, withOutHighWaysCallbackObjRef);
}

function withHighWaysCallbackObjRef(response, status) {
    if (response === null) {
        console.log("withHighWaysCallbackObjRef");
        console.log(status);
    }
    lastObjRef.invokeMethodAsync('SetGoogleDistance', response.rows[0].elements[0]);
}

function withOutHighWaysCallbackObjRef(response, status) {
    if (response === null) {
        console.log("withOutHighWaysCallbackObjRef");
        console.log(status);
    }
    lastObjRef.invokeMethodAsync('SetGoogleDistance', response.rows[0].elements[0]);
}

function enableTextbox(chkId, txtId) {
    let forceUncked = false;
    if (chkId !== "needsRestaurant") {
        if (document.getElementById("needsRestaurant").checked) {
            document.getElementById(chkId).checked = false;
            forceUncked = true;
        }
    }
    else
        if (document.getElementById("needsMuseum").checked) {
            document.getElementById(chkId).checked = false;
            forceUncked = true;
        }

    if (document.getElementById(chkId).checked) {
        if (city_initialized == true && document.getElementById('city_search').value != '') {
            document.getElementById(txtId).disabled = false;
        }
    }
    else {
        if (!forceUncked) {
            document.getElementById(txtId).disabled = true;
        }
    }
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

function showAlert(message) {
    return alert(message);
}