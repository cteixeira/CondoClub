(function ($) {
    $.fn.AddressPicker = function (options) {

        var defaults = {
            Latitude: null,
            Longitude: null,
            Zoom: 5,
            LocationFoundZoom: 16,
            MapTypeID: google.maps.MapTypeId.ROADMAP,
            SearchInputSelector: '#searchAddress',
            EnderecoInputSelector: '#Endereco',
            CodigoPostalInputSelector: '#CodigoPostal',
            LocalidadeInputSelector: '#Localidade',
            CidadeInputSelector: '#Cidade',
            EstadoInputSelector: '#Estado',
            PaisSelectSelector: '#PaisID',
            LatitudeInputSelector: '#Latitude',
            LongitudeInputSelector: '#Longitude',
            ContentorSelector: '.form'
        };

        var settings = $.extend(defaults, options);
        var self = $(this);
        var contentor = self.closest(settings.ContentorSelector);
        var defaultCenter = new google.maps.LatLng(-23.5489433, -46.6388182);
        var center;

        var searchInput = contentor.find(settings.SearchInputSelector);
        var endereco = contentor.find(settings.EnderecoInputSelector);
        var codigoPostal = contentor.find(settings.CodigoPostalInputSelector);
        var localidade = contentor.find(settings.LocalidadeInputSelector);
        var cidade = contentor.find(settings.CidadeInputSelector);
        var estado = contentor.find(settings.EstadoInputSelector);
        var pais = contentor.find(settings.PaisSelectSelector);
        var latitude = contentor.find(settings.LatitudeInputSelector);
        var longitude = contentor.find(settings.LongitudeInputSelector);

        Initialize = function () {
            if (settings.Latitude && settings.Longitude)
                center = new google.maps.LatLng(settings.Latitude, settings.Longitude);

            var mapOptions;

            if (center) {
                mapOptions = {
                    center: center,
                    zoom: settings.LocationFoundZoom,
                    mapTypeId: settings.MapTypeID
                };
            }
            else {
                mapOptions = {
                    center: defaultCenter,
                    zoom: settings.Zoom,
                    mapTypeId: settings.MapTypeID
                };
            }


            var map = new google.maps.Map(self[0], mapOptions);
            var geocoder = new google.maps.Geocoder();
            var autocomplete = new google.maps.places.Autocomplete(searchInput[0]);
            var marker = new google.maps.Marker({
                map: map,
                draggable: true
            });

            if (center) {
                marker.setPosition(center);
            }

            // eventos
            autocomplete.bindTo('bounds', map);

            google.maps.event.addListener(marker, 'dragend', function () {
                geocoder.geocode(
                    { latLng: marker.getPosition() },
                    function (responses) {
                        if (responses && responses.length > 0) {
                            searchInput.val(responses[0].formatted_address);
                            RefreshData(responses[0], map, marker);
                        }
                    }
                );
            });

            google.maps.event.addListener(autocomplete, 'place_changed', function () {
                var place = GetPlaceAutocomplete(autocomplete);
                RefreshData(place, map, marker);
                SetMarker(marker, place);
            });

            searchInput.keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
            });
        }


        GetPlaceAutocomplete = function (autocomplete) {
            return autocomplete.getPlace([
                "street_address",
                "locality",
                "sublocality",
                "postal_code",
                "country",
                "administrative_area1",
                "administrative_area2"
            ]);
        }

        RefreshData = function (place, map, marker) {
            if (!place.geometry)
                return;

            if (place.geometry.viewport) {
                map.fitBounds(place.geometry.viewport);
            } else {
                map.setCenter(place.geometry.location);
                map.setZoom(settings.LocationFoundZoom);
            }

            ParseAddress(place);
        }

        ParseAddress = function (place) {
            // limpar valores anteriores
            endereco.val('');
            codigoPostal.val('');
            localidade.val('');
            estado.val('');
            cidade.val('');
            pais.val('');
            latitude.val('');
            longitude.val('');

            $(place.address_components).each(function () {
                if (this.types.indexOf("route") != -1) {
                    endereco.val(this.long_name);
                }

                if (this.types.indexOf("postal_code") != -1 &&
                    this.types.indexOf("postal_code_prefix") == -1) {
                    codigoPostal.val(this.long_name);
                }

                if (this.types.indexOf("locality") != -1) {
                    localidade.val(this.long_name);
                }

                if (this.types.indexOf("administrative_area_level_1") != -1) {
                    estado.val(this.long_name);
                }

                if (this.types.indexOf("administrative_area_level_2") != -1) {
                    cidade.val(this.long_name);
                }

                if (this.types.indexOf("country") != -1) {
                    pais.find("option:contains('" + this.long_name + "')").attr('selected', 'selected');
                }
            });

            if(cidade.val() == '')
                cidade.val(localidade.val());

            latitude.val(place.geometry.location.lat());
            longitude.val(place.geometry.location.lng());
        }

        SetMarker = function (marker, place) {
            marker.setIcon(({
                url: place.icon,
                size: new google.maps.Size(71, 71),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(17, 34),
                scaledSize: new google.maps.Size(35, 35)
            }));
            marker.setPosition(place.geometry.location);
        }


        Initialize();
    }
})(jQuery);


(function ($) {
    $.fn.AddressPickerReadOnly = function (options) {
        
        var defaults = {
            Latitude: null,
            Longitude: null,
            Zoom: 13,
            MapTypeID: google.maps.MapTypeId.ROADMAP,
            Animation: google.maps.Animation.DROP
        };

        var settings = $.extend(defaults, options);
        var self = $(this);

        Initialize = function () {
            var latlng = new google.maps.LatLng(settings.Latitude, settings.Longitude);

            var map = new google.maps.Map(
                self[0],
                {
                    scrollwheel: false,
                    draggable: false,
                    zoom: settings.Zoom,
                    center: latlng,
                    mapTypeId: settings.MapTypeID
                }
            );

            var marker = new google.maps.Marker(
            {
                map: map,
                animation: settings.Animation,
                position: latlng
            });

            google.maps.event.addDomListener(window, 'load', Initialize);
        }

        Initialize();
    }
})(jQuery);