
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

    // Create the search box and link it to the UI element.
    let input = document.getElementById('pac-input');
    let searchBox = new google.maps.places.SearchBox(input);

    map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

    // Bias the SearchBox results towards current map's viewport.
    map.addListener('bounds_changed', function () {
        searchBox.setBounds(map.getBounds());
    });

    //prevent submiting register sensor form when pressing Enter key
    google.maps.event.addDomListener(input, 'keydown', function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();
        }
    });

    let markers = [];
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
        });
        map.fitBounds(bounds);
    });
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
