document.getElementsByTagName("body")[0].classList.add("gallery-page");



var $container = $('#mix-container'), // mixitup container
    $toList = $('.to-list'), // list view button
    $toGrid = $('.to-grid'); // list view button

// Instantiate MixItUp
$container.mixItUp({
    controls: {
        enable: false // we won't be needing these
    },
    animation: {
        duration: 400,
        effects: 'fade translateZ(-360px) stagger(45ms)',
        easing: 'ease'
    },
    callbacks: {
        onMixFail: function () { }
    }
});

$toList.on('click', function () {
    if ($container.hasClass('list')) {
        return
    }
    $container.mixItUp('changeLayout', {
        display: 'block',
        containerClass: 'list'
    }, function (state) {
        // callback function
    });
});
$toGrid.on('click', function () {
    if ($container.hasClass('grid')) {
        return
    }
    $container.mixItUp('changeLayout', {
        display: 'inline-block',
        containerClass: 'grid'
    }, function (state) {
        // callback function
    });
});

// Add Gallery Item to Lightbox
$('.mix img').magnificPopup({
    type: 'image',
    callbacks: {
        beforeOpen: function (e) {
            // we add a class to body to indicate overlay is active
            // We can use this to alter any elements such as form popups
            // that need a higher z-index to properly display in overlays
            $('body').addClass('mfp-bg-open');

            // Set Magnific Animation
            this.st.mainClass = 'mfp-zoomIn';

            // Inform content container there is an animation
            this.contentContainer.addClass('mfp-with-anim');
        },
        afterClose: function (e) {

            setTimeout(function () {
                $('body').removeClass('mfp-bg-open');
                $(window).trigger('resize');
            }, 1000)

        },
        elementParse: function (item) {
            // Function will fire for each target element
            // "item.el" is a target DOM element (if present)
            // "item.src" is a source that you may modify
            item.src = item.el.attr('src');
        },
    },
    overflowY: 'scroll',
    removalDelay: 200, //delay removal by X to allow out-animation
    prependTo: $('#content_wrapper')
});
