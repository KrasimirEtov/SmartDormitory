
let marker;

function initMap() {
    //var sofiaCenterOfTheMap = { lat: 42.6977, lng: 23.3219 };
    const options_googlemaps = {
        minZoom: 4,
        zoom: 11,
        center: new google.maps.LatLng(42.6977, 23.3219), //sofia coords
        maxZoom: 18,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        streetViewControl: false
    }

    const map = new google.maps.Map(document.getElementById('mapModal'), options_googlemaps);

    // This event listener calls methods when the map is clicked.
    google.maps.event.addListener(map, 'click', function (event) {
        deleteMarker();
        addMarker(event.latLng, map);
    });
}

function addMarker(location, map) {

    $('#lat').val(location.lat().toFixed(5));
    $('#lon').val(location.lng().toFixed(5));

    marker = new google.maps.Marker({
        position: location,
        map: map,
        title: 'My sensor address',
        animation: google.maps.Animation.BOUNCE
    });
}

function deleteMarker() {
    if (marker) {
        if (marker.getMap() !== null) {
            marker.setMap(null)
        }
    }
}
