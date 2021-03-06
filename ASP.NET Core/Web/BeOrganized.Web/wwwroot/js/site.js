﻿(function ($) {
    "use strict";
  

    if ($('section').is('.removeContainer') || $('div').is('.removeContainer')) {
        $('main').removeClass('container');
    };

    // Preloader 
    $(window).on('load', function () {
        if ($('#preloader').length) {
            $('#preloader').delay(5).fadeOut('slow', function () {
                $(this).remove();
            });
        }
    });

    $(window).on("load", function () {
        //$('#deleteButton').click(function (e) { e.stopPropagation() });
        $('#hoverDetails').click(function () {
            $('#hoverDetails').hide();
        });
        $('#closeButtonDetails').click(function () {
            $('#hoverDetails').hide();
        });
    });

    // Notification alert Fade out
    $(".notification").delay(4000).fadeOut('slow');

    // Smooth scroll for the navigation and links with .scrollto classes
    $('.main-nav a, .mobile-nav a, .scrollto').on('click', function () {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
            var target = $(this.hash);
            if (target.length) {
                var top_space = 0;

                if ($('#header').length) {
                    top_space = $('#header').outerHeight();

                    if (!$('#header').hasClass('header-scrolled')) {
                        top_space = top_space - 40;
                    }
                }

                $('html, body').animate({
                    scrollTop: target.offset().top - top_space
                }, 1500, 'easeInOutExpo');

                if ($(this).parents('.main-nav, .mobile-nav').length) {
                    $('.main-nav .active, .mobile-nav .active').removeClass('active');
                    $(this).closest('li').addClass('active');
                }

                if ($('body').hasClass('mobile-nav-active')) {
                    $('body').removeClass('mobile-nav-active');
                    $('.mobile-nav-toggle i').toggleClass('fa-times fa-bars');
                    $('.mobile-nav-overly').fadeOut();
                }
                return false;
            }
        }
    });

    // Navigation active state on scroll
    var nav_sections = $('section');
    var main_nav = $('.main-nav, .mobile-nav');
    var main_nav_height = $('#header').outerHeight();

    $(window).on('scroll', function () {
        var cur_pos = $(this).scrollTop();

        nav_sections.each(function () {
            var top = $(this).offset().top - main_nav_height,
                bottom = top + $(this).outerHeight();

            if (cur_pos >= top && cur_pos <= bottom) {
                main_nav.find('li').removeClass('active');
                main_nav.find('a[href="#' + $(this).attr('id') + '"]').parent('li').addClass('active');
            }
        });
    });

})(jQuery);

