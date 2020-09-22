(function($) {
	'use strict';


    //======================
    // Preloder
    //======================
    $(window).on('load', function() {
        $('#status').fadeOut();
        $('#preloader').delay(500).fadeOut('slow');
        $('body').delay(500).css({'overflow':'visible'});
    });

        

    // ====================================
    //  Slider, testimonial - owl carousel
    // ====================================

	$('.slider-wrap, .testimonial').owlCarousel({
	    dots: true,
        loop:true,
	    margin:10,
	    nav:false,
	    responsive:{
	        0:{
	            items:1
	        },
	        600:{
	            items:1
	        },
	        1000:{
	            items:1
	        }
	    }
	});


    //// ====================================
    ////  Partners logo - owl carousel
    //// ====================================

    //$('.partners-logo').owlCarousel({
    //    dots: false,
    //    loop: false,
    //    margin: 0,
    //    nav: false,
    //    responsive:{
    //        0:{
    //            items: 2
    //        },
    //        400:{
    //            items:3
    //        },
    //        576:{
    //            items:4
    //        },
    //        768:{
    //            items:5
    //        },
    //        1200:{
    //            items:7
    //        }
    //    }
    //});


    // ====================================
    //  Product preview - owl carousel
    // ====================================

    $('.preview-caro').owlCarousel({
        dots: false,
        items:1,
        nav: true,
        navText: ['<i class="fas fa-angle-left"></i>', '<i class="fas fa-angle-right"></i>'],
        smartSpeed: 500,
        thumbs: true,
        thumbsPrerendered: true
    });


    // ====================================
    //  Custom dropdown
    // ====================================
    if($(window).width() < 768) {
        $('.dropdown').on('click', function() {
            $(this).children('ul').slideToggle("fast");
        });
    }
    


    // ====================================
    //  Grid/List view switcher
    // ====================================
	$('.grid-view').on('click', function(e) {
		e.preventDefault();
        $('.list-view').removeClass('active');
        $(this).addClass('active');
		$('.product-layout').removeClass('list-layout');
		$('.product-layout').addClass('grid-layout');
		$('.grid-layout').children().removeClass('col-lg-12 col-md-12 col-sm-12');
		$('.grid-layout').children().addClass('col-lg-3 col-md-4 col-sm-6');
	});
	$('.list-view').on('click', function(e) {
		e.preventDefault();
        $('.grid-view').removeClass('active');
        $(this).addClass('active');
		$('.product-layout').removeClass('grid-layout');
		$('.product-layout').addClass('list-layout');
		$('.list-layout').children().removeClass('col-lg-3 col-md-4 col-sm-6');
		$('.list-layout').children().addClass('col-lg-12 col-md-12 col-sm-12');
	});


	//=========================
    // Price Range Slider
    //=========================
    if ($("#slider-range").length > 0) 
    {
        $("#slider-range").slider({
            range: true,
            min: 0,
            max: 700,
            values: [0, 560],
            slide: function (event, ui) {
                $("#minamount").html("$" + ui.values[ 0 ]);
                $("#maxamount").html("$" + ui.values[ 1 ]);
            }
        });
        $("#minamount").html("$" + $("#slider-range").slider("values", 0));
        $("#maxamount").html("$" + $("#slider-range").slider("values", 1));

    };


    //=================================
    // Add/Minus Quantity
    //=================================
    $('.incressQnt').on('click',function(){
        var $qty = $(this).closest('div').find('.qnttinput');
        var currentVal = parseInt($qty.val());
        if (!isNaN(currentVal)) {
            $qty.val(currentVal + 1);
        }
    });
    $('.decressQnt').on('click',function(){
        var $qty = $(this).closest('div').find('.qnttinput');
        var currentVal = parseInt($qty.val());
        if (!isNaN(currentVal) && currentVal > 0) {
            $qty.val(currentVal - 1);
        }
    });


    //=================================
    // CountDown Timer
    //=================================
    if ($(".clock").length > 0) {
        $('.clock').countdown('2020/10/10').on('update.countdown', function(event) {
          var $this = $(this).html(event.strftime(''
            + '<p><span>%-d</span> Day%!d</p> '
            + '<p><span>%H</span> Hour%!d</p> '
            + '<p><span>%M</span> Mins%!d</p> '
            + '<p><span>%S</span> Secs%!d</p>'));
        });
    }


    // ====================================
    //  Toggle mini cart
    // ====================================
    // Toggle mini cart
    if($(".cart-btn").length > 0) {
        $(".cart-btn").on('click', function(e) {
            e.preventDefault();
            $(this).toggleClass('active');
            $(this).siblings('.mini-cart-con').toggleClass('show');
        })
    }


    // ====================================
    //  Producto filtering
    // ====================================
    $(".product-filter li").on('click', function () {
        $(".product-filter li").removeClass('active');
        $(this).addClass('active');
        var selector = $(this).attr('data-filter');
        $(".product-wrap").isotope({
            filter: selector
        });
    });


    // ====================================
    //  Mobile menu 
    // ====================================
    if($('.mblmenu_toggler').length > 0) {
        $('.mblmenu_toggler').on('click', function() {
            $('.mblmenu-container').toggleClass('opened');
            $('.mblmenu-overlay').toggleClass('active');
        });
    }


    //// ====================================
    ////  Newsletter Popup
    //// ====================================
    //if($('.newsletter-popup').length > 0) {
    //    setTimeout(function(){
    //        $('.newsletter-popup').addClass('show');
    //        $('.mblmenu-overlay').addClass('active');
    //    }, 3000);
    //    $('.popup-close').on('click', function() {
    //        $('.newsletter-popup').removeClass('show');
    //        $('.mblmenu-overlay').removeClass('active');
    //    });
    //}


    // ====================================
    //  Search bar 
    // ====================================
    // $(".search-btn").on("click", function(e) {
    //     $(this).children('i').toggleClass('fa-times');
    //     $(".header-search").toggleClass("show");
    // });
    


    //// ====================================
    ////  Contact form
    //// ====================================
    //$('#contact-form').on("submit", function () {
    //    var action = $(this).attr('action');
    //    $("#message").slideUp(750, function () {
    //        $('#message').hide();
    //        $('#submit')
    //                .after('<img src="images/ajax-loader.gif" class="loader" />')
    //                .attr('disabled', 'disabled');
    //        $.post(action, {
    //            name: $('#name').val(),
    //            email: $('#email').val(),
    //            // subject: $('#subject').val(),
    //            comments: $('#comments').val()
    //        },
    //        function (data) {
    //            document.getElementById('message').innerHTML = data;
    //            $('#message').slideDown('slow');
    //            $('#contact-form img.loader').fadeOut('slow', function () {
    //                $(this).remove()
    //            });
    //            $('#submit').removeAttr('disabled');
    //            if (data.match('success') != null)
    //                $('#contact-form').show('slow');
    //        }
    //        );

    //    });
    //    return false;
    //});


    //// ====================================
    ////  Mailchimp filtering
    //// ====================================
    //$('#subscribe').ajaxChimp({
    //    url: 'https://themencoder.us11.list-manage.com/subscribe/post?u=c891be02cd2b3f7ebaf6b0fef&id=a9e3d365f5'
    //});


    // ====================================
    //  GOOGLE MAP 
    // ====================================
    function initGoogleMaps() {
        // Basic options for a simple Google Map
        // For more options see: https://developers.google.com/maps/documentation/javascript/reference#MapOptions
        var mapOptions = {
            // How zoomed in you want the map to start at (always required)
            zoom: 11,
            scrollwheel: false,
            navigationControl: false,
            mapTypeControl: false,
            scaleControl: false,
            draggable: false,
            // The latitude and longitude to center the map (always required)
            center: new google.maps.LatLng(40.7127, -74.0059), // New york

            // How you would like to style the map.
            // This is where you would paste any style found on Snazzy Maps.
            styles: [{"featureType": "water", "elementType": "geometry", "stylers": [{"color": "#e9e9e9"}, {"lightness": 17}]}, {"featureType": "landscape", "elementType": "geometry", "stylers": [{"color": "#f5f5f5"}, {"lightness": 20}]}, {"featureType": "road.highway", "elementType": "geometry.fill", "stylers": [{"color": "#ffffff"}, {"lightness": 17}]}, {"featureType": "road.highway", "elementType": "geometry.stroke", "stylers": [{"color": "#ffffff"}, {"lightness": 29}, {"weight": 0.2}]}, {"featureType": "road.arterial", "elementType": "geometry", "stylers": [{"color": "#ffffff"}, {"lightness": 18}]}, {"featureType": "road.local", "elementType": "geometry", "stylers": [{"color": "#ffffff"}, {"lightness": 16}]}, {"featureType": "poi", "elementType": "geometry", "stylers": [{"color": "#f5f5f5"}, {"lightness": 21}]}, {"featureType": "poi.park", "elementType": "geometry", "stylers": [{"color": "#dedede"}, {"lightness": 21}]}, {"elementType": "labels.text.stroke", "stylers": [{"visibility": "on"}, {"color": "#ffffff"}, {"lightness": 16}]}, {"elementType": "labels.text.fill", "stylers": [{"saturation": 36}, {"color": "#333333"}, {"lightness": 40}]}, {"elementType": "labels.icon", "stylers": [{"visibility": "off"}]}, {"featureType": "transit", "elementType": "geometry", "stylers": [{"color": "#f2f2f2"}, {"lightness": 19}]}, {"featureType": "administrative", "elementType": "geometry.fill", "stylers": [{"color": "#fefefe"}, {"lightness": 20}]}, {"featureType": "administrative", "elementType": "geometry.stroke", "stylers": [{"color": "#fefefe"}, {"lightness": 17}, {"weight": 1.2}]}]
        };

        // Get the HTML DOM element that will contain your map
        // We are using a div with id="map" seen below in the <body>
        var mapElement = document.getElementById('map');

        // Create the Google Map using our element and options defined above
        var map = new google.maps.Map(mapElement, mapOptions);

        // Let's also add a marker while we're at it
        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(40.7127, -74.0059),
            map: map,
            title: 'Find us here!'
        });
    }
    if ($("#map").length > 0) {
        // When the window has finished loading create our google map below
        var googleMaps = google.maps.event.addDomListener(window, 'load', initGoogleMaps);
    }   



})(jQuery);