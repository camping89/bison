(function ($) {
    "use strict"; // Start of use strict
    //Tag Toggle
    function toggle_tab() {
        if ($('.toggle-tab').length > 0) {
            $('.toggle-tab').each(function () {
                /* $(this).find('.item-toggle-tab').first().find('.toggle-tab-content').show(); */
                $('.toggle-tab-title').on('click', function () {
                    $(this).parent().siblings().removeClass('active');
                    $(this).parent().toggleClass('active');
                    $(this).parents('.toggle-tab').find('.toggle-tab-content').slideUp();
                    $(this).next().stop(true, false).slideToggle();
                });
            });
        }
    }
    //Popup Wishlist
    function popup_wishlist() {
        $('.wishlist-link').on('click', function (event) {
            event.preventDefault();
            $('.wishlist-mask').fadeIn();
            var counter = 10;
            var popup;
            popup = setInterval(function () {
                counter--;
                if (counter < 0) {
                    clearInterval(popup);
                    $('.wishlist-mask').hide();
                } else {
                    $(".wishlist-countdown").text(counter.toString());
                }
            }, 1000);
        });
    }
    //Custom ScrollBar
    function custom_scroll() {
        if ($('.custom-scroll').length > 0) {
            $('.custom-scroll').each(function () {
                $(this).mCustomScrollbar({
                    scrollButtons: {
                        enable: true
                    }
                });
            });
        }
    }
    //Offset Menu
    function offset_menu() {
        if ($(window).width() > 767) {
            $('.main-nav .sub-menu').each(function () {
                var wdm = $(window).width();
                var wde = $(this).width();
                var offset = $(this).offset().left;
                var tw = offset + wde;
                if (tw > wdm) {
                    $(this).addClass('offset-right');
                }
            });
        } else {
            return false;
        }
    }
    //Fixed Header
    function fixed_header() {
        if ($('.header-ontop').length > 0) {
            if ($(window).width() > 1023) {
                var ht = $('#header').height();
                var st = $(window).scrollTop();
                if (st > ht) {
                    $('.header-ontop').addClass('fixed-ontop');
                } else {
                    $('.header-ontop').removeClass('fixed-ontop');
                }
            } else {
                $('.header-ontop').removeClass('fixed-ontop');
            }
        }
    }
    //Slider Background
    function background() {
        $('.bg-slider .item-slider').each(function () {
            var src = $(this).find('.banner-thumb a img').attr('src');
            $(this).css('background-image', 'url("' + src + '")');
        });
    }
    function animated() {
        $('.banner-slider .owl-item').each(function () {
            var check = $(this).hasClass('active');
            if (check == true) {
                $(this).find('.animated').each(function () {
                    var anime = $(this).attr('data-animated');
                    $(this).addClass(anime);
                });
            } else {
                $(this).find('.animated').each(function () {
                    var anime = $(this).attr('data-animated');
                    $(this).removeClass(anime);
                });
            }
        });
    }
    function slick_animated() {
        $('.banner-slider .item-slider').each(function () {
            var check = $(this).hasClass('slick-active');
            if (check == true) {
                $(this).find('.animated').each(function () {
                    var anime = $(this).attr('data-animated');
                    $(this).addClass(anime);
                });
            } else {
                $(this).find('.animated').each(function () {
                    var anime = $(this).attr('data-animated');
                    $(this).removeClass(anime);
                });
            }
        });
    }
    function slick_control() {
        $('.slick-slider').each(function () {
            $(this).find('.slick-prev .slick-caption').html('<div class="slick-thumb slick-prev-img"></div>' + '<div class="slick-info"><h3 class="title12">Prev Slide</h3><p class="desc"></p></div>');
            $(this).find('.slick-next .slick-caption').html('<div class="slick-thumb slick-next-img"></div>' + '<div class="slick-info"><h3 class="title12">Next Slide</h3><p class="desc"></p></div>');
            $(this).find('.slick-prev-img').css('background-image', 'url("' + $('.slick-active').prev().find('.banner-thumb a img').attr('src') + '")');
            $(this).find('.slick-next-img').css('background-image', 'url("' + $('.slick-active').next().find('.banner-thumb a img').attr('src') + '")');
            $(this).find('.slick-prev .desc').html($('.slick-active').prev().find('.desc-control').html());
            $(this).find('.slick-next .desc').html($('.slick-active').next().find('.desc-control').html());
        });
    }
    //Detail Gallery
    function detail_gallery(numbercount) {
        if ($('.detail-gallery').length > 0) {
            alert($('.detail-gallery').length);
            $('.detail-gallery').each(function () {
                $(this).find(".carousel").jCarouselLite({
                    btnNext: $(this).find(".gallery-control .next"),
                    btnPrev: $(this).find(".gallery-control .prev"),
                    speed: 800,
                    visible: numbercount + 0.5,
                    circular: false
                });
                //Elevate Zoom
                $('.detail-gallery').find('.mid img').elevateZoom({
                    zoomType: "inner",
                    cursor: "crosshair",
                    zoomWindowFadeIn: 500,
                    zoomWindowFadeOut: 850
                });
                $(this).find(".carousel a").on('click', function (event) {
                    event.preventDefault();
                    $(this).parents('.detail-gallery').find(".carousel a").removeClass('active');
                    $(this).addClass('active');
                    var z_url = $(this).find('img').attr("src");
                    $(this).parents('.detail-gallery').find(".mid img").attr("src", z_url);
                    $('.zoomWindow').css('background-image', 'url("' + z_url + '")');
                });
            });
        }
    }
    //Menu Responsive
    function menu_responsive() {
        if ($(window).width() < 768) {
            if ($('.btn-toggle-mobile-menu').length > 0) {
                return false;
            } else {
                $('.main-nav li.menu-item-has-children,.main-nav li.has-mega-menu').append('<span class="btn-toggle-mobile-menu"></span>');
                $('.main-nav .btn-toggle-mobile-menu').on('click', function (event) {
                    $(this).toggleClass('active');
                    $(this).prev().stop(true, false).slideToggle();
                });
            }
        } else {
            $('.btn-toggle-mobile-menu').remove();
            $('.main-nav .sub-menu,.main-nav .mega-menu').slideDown();
        }
    }
    //Document Ready
    jQuery(document).ready(function () {
        //Switch Register
        $('.login-to-register').on('click', function (event) {
            event.preventDefault();
            $(this).toggleClass('login-status');
            if ($(this).hasClass('login-status')) {
                $(this).text($(this).attr('data-login'));
                $(this).parents('.register-content-box').find('.block-login').hide();
                $(this).parents('.register-content-box').find('.block-register').show();
            } else {
                $(this).text($(this).attr('data-register'));
                $(this).parents('.register-content-box').find('.block-login').show();
                $(this).parents('.register-content-box').find('.block-register').hide();
            }
        });
        //Menu Responsive
        $('.toggle-mobile-menu').on('click', function (event) {
            event.preventDefault();
            $(this).parents('.main-nav').toggleClass('active');
        });
        //Service Hover
        if ($('.list-service').length > 0) {
            $('.list-service').each(function () {
                $(this).find('.item-service').on('mouseover', function () {
                    $(this).parents('.list-service').find('.item-service').removeClass('active');
                    $(this).addClass('active');
                });
                $(this).on('mouseout', function () {
                    $(this).find('.item-service').removeClass('active');
                    $(this).find('.item-active').addClass('active');
                });
            });
        }
        //New Filter price
        //if ($('.new-filter').length > 0) {

        //    //var urlParams = new URLSearchParams(location.search);
        //    var pf = getParameterByName("pf", window.location.href);
        //    var pt = getParameterByName("pt", window.location.href);
        //    if (pf == null) {
        //        pf = 0;
        //    }
        //    if (pt == null || pt === 0) {
        //        pt = 10000000;
        //    }
        //    var slider = $("#slider-range").slider({
        //        range: true,
        //        step: 100000,
        //        min: 0,
        //        max: 10000000,
        //        values: [pf, pt],
        //        slide: function (event, ui) {
        //            $(".filter-from-price").val(ui.values[0].format());
        //            $(".filter-to-price").val(ui.values[1].format());

        //        },
        //        change: function (event, ui) {
        //            minPrice = ui.values[0];
        //            maxPrice = ui.values[1];
        //            SearchProductAjax();
        //        }
        //    });

        //    $(".filter-from-price").val($("#slider-range").slider("values", 0).format());
        //    $(".filter-to-price").val($("#slider-range").slider("values", 1).format());

        //    $(".filter-from-price").on("change", function () {
        //        slider.slider("option", "values", [$(this).val().removeComma(), $(".filter-to-price").val().removeComma()]);

        //        var strPrice = $(this).val().toInt().format();
        //        $(this).val(strPrice);
        //    });

        //    $(".filter-to-price").on("change", function () {
        //        slider.slider("option", "values", [$(".filter-from-price").val().removeComma(), $(this).val().removeComma()]);

        //        var strPrice = $(this).val().toInt().format();
        //        $(this).val(strPrice);
        //    });
            
        //    //keypress

        //}

        //Filter Price
        //if($('.range-filter').length>0){
        //    var urlParams = new URLSearchParams(location.search);
        //    var pf = urlParams.get("pf");
        //    var pt = urlParams.get("pt");
        //    if (pf == null) {
        //        pf = 0;
        //    }
        //    if (pt == null || pt === 0) {
        //        pt = 800;
        //    }
        //    $('.range-filter').each(function(){
        //        $(this).find( ".slider-range" ).slider({
        //            range: true,
        //            min: 0,
        //            step: 100,
        //            max: 800,
        //            values: [ pf, pt ],
        //            slide: function( event, ui ) {
        //                $(this).parents('.range-filter').find( ".amount" ).html( '<span class="startprice">'+ui.values[ 0 ].format()+'</span>' + '<span class="endprice">' + ui.values[ 1 ].format()+'</span>');
        //            }
        //        });
        //        $(this).find( ".amount" ).html('<span class="startprice">'+$(this).find( ".slider-range" ).slider( "values", 0 )+'</span>' + '<span class="endprice">'+$(this).find( ".slider-range" ).slider( "values", 1 )+'</span>');
        //        $(".startprice").html($(".startprice").text().toInt().format());
        //        $(".endprice").html($(".endprice").text().toInt().format());
        //    });
        //}


        //Qty Up-Down
        $('.detail-qty').each(function () {
            var qtyval = parseInt($(this).find('.qty-val').text(), 10);
            $(this).find('.qty-up').on('click', function (event) {
                event.preventDefault();
                qtyval = qtyval + 1;
                $('.qty-val').text(qtyval);
            });
            $(this).find('.qty-down').on('click', function (event) {
                event.preventDefault();
                qtyval = qtyval - 1;
                if (qtyval > 1) {
                    $('.qty-val').text(qtyval);
                } else {
                    qtyval = 1;
                    $('.qty-val').text(qtyval);
                }
            });
        });
        //Detail Gallery
        //detail_gallery();
        //Wishlist Popup
        popup_wishlist();
        //Menu Responsive 
        menu_responsive();
        //Offset Menu
        offset_menu();
        //Toggle Tab
        toggle_tab();
        //Animate
        if ($('.wow').length > 0) {
            new WOW().init();
        }
        //Video Light Box
        if ($('.btn-video').length > 0) {
            $('.btn-video').fancybox({
                openEffect: 'none',
                closeEffect: 'none',
                prevEffect: 'none',
                nextEffect: 'none',

                arrows: false,
                helpers: {
                    media: {},
                    buttons: {}
                }
            });
        }
        //Light Box
        if ($('.fancybox').length > 0) {
            $('.fancybox').fancybox();
        }
        //Back To Top
        $('.scroll-top').on('click', function (event) {
            event.preventDefault();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        });
    });
    //Window Load
    jQuery(window).on('load', function () {
        //Owl Carousel
        if ($('.wrap-item').length > 0) {
            $('.wrap-item').each(function () {
                var data = $(this).data();
                $(this).owlCarousel({
                    addClassActive: true,
                    stopOnHover: true,
                    itemsCustom: data.itemscustom,
                    autoPlay: data.autoplay,
                    transitionStyle: data.transition,
                    paginationNumbers: data.paginumber,
                    beforeInit: background,
                    afterAction: animated,
                    navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
                });
            });
        }
        //Slick Slider
        if ($('.banner-slider .slick').length > 0) {
            $('.banner-slider .slick').each(function () {
                $(this).slick({
                    dots: true,
                    infinite: true,
                    slidesToShow: 1,
                    prevArrow: '<div class="slick-prev"><div class="slick-caption"></div><div class="slick-nav"></div></div>',
                    nextArrow: '<div class="slick-next"><div class="slick-caption"></div><div class="slick-nav"></div></div>'
                });
                slick_control();
                $('.slick').on('afterChange', function (event) {
                    slick_control();
                    slick_animated();
                });
            });
        }
        //Day Countdown
        if ($('.days-countdown').length > 0) {
            $(".days-countdown").TimeCircles({
                fg_width: 0.05,
                bg_width: 0,
                text_size: 0,
                circle_bg_color: "transparent",
                time: {
                    Days: {
                        show: true,
                        text: "D",
                        color: "#fff"
                    },
                    Hours: {
                        show: true,
                        text: "H",
                        color: "#fff"
                    },
                    Minutes: {
                        show: true,
                        text: "M",
                        color: "#fff"
                    },
                    Seconds: {
                        show: true,
                        text: "S",
                        color: "#fff"
                    }
                }
            });
        }
        //Count Down Master
        if ($('.countdown-master').length > 0) {
            $('.countdown-master').each(function () {
                $(this).FlipClock(65100, {
                    clockFace: 'HourlyCounter',
                    countdown: true,
                    autoStart: true,
                });
            });
        }
    });
    //Window Resize
    jQuery(window).on('resize', function () {
        offset_menu();
        fixed_header();
        //detail_gallery();
        //Menu Responsive 
        menu_responsive();
    });
    //Window Scroll
    jQuery(window).on('scroll', function () {
        //Scroll Top
        if ($(this).scrollTop() > $(this).height()) {
            $('.scroll-top').addClass('active');
        } else {
            $('.scroll-top').removeClass('active');
        }
        //Fixed Header
        fixed_header();
    });
})(jQuery); // End of use strict


Number.prototype.format = function (n, x) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
    return this.toFixed(Math.max(0, ~~n)).replace(new RegExp(re, 'g'), '$&,');
};
String.prototype.toInt = function () {
    if (this != null || this != "") {
        var c = this.replace(/,/g, "");
        return parseInt(c);
    }
    return 0;
}

String.prototype.toFloat = function () {
    if (this != null || this != "") {
        var c = this.replace(/,/g, "");
        return parseFloat(c);
    }
    return 0;
}


String.prototype.removeComma = function () {
    if (this != null || this != "") {
        var c = this.replace(/,/g, "");
        return c;
    }
    return 0;
}

jQuery.expr[':'].contains = function(a, i, m) {
    return jQuery(a).text().toUpperCase()
        .indexOf(m[3].toUpperCase()) >= 0;
};

/*Ajax search products*/
//$(document).ready(function () {
    
//    if ($(".filter-ajax-result").length > 0) {
//        if ($("#products-pagesize").length > 0) {
//            $("#products-orderby")[0].onchange = null;
//        }

//        if ($("#products-pagesize").length > 0) {
//            $("#products-pagesize")[0].onchange = null;
//        }

//        $(document).on('change', '#products-orderby,#products-pagesize', function (event) {
//            event.preventDefault();
//            sorting = getParameterByName('orderby', $("#products-orderby").val()) != null ? getParameterByName('orderby', $("#products-orderby").val()) : 0;
//            pagesize = getParameterByName('pagesize', $("#products-pagesize").val()) != null ? getParameterByName('pagesize', $("#products-pagesize").val()) : 0;
//            SearchProductAjax();
//        });
//        $(document).on('click', '.individual-page a,.next-page a,.previous-page a ,.last-page a, .first-page a', function (event) {
//            event.preventDefault();
    
//            SearchProductAjax(getParameterByName("pagenumber", $(this)[0].href) == null ? 0 : getParameterByName("pagenumber", $(this)[0].href));
//        });

//        $(document).on('click', '.viewmode-icon', function (event) {
//            event.preventDefault();
//            selectViewMode($(this)[0].title);
//        });
        
//        $(document).on('click', '.checkbox-manufacture', function (event) {
//            //event.preventDefault();
//            SearchProductAjax();
//        });
//    }
//});

//function selectViewMode(type) {
//    //if grid is selected and grid is not active
//    if ($('.viewmode-icon.grid-view.active').length == 0) {
//        $('.viewmode-icon.list-view.active').removeClass("active");
//        $('.viewmode-icon.grid-view').addClass("active");
//        viewMode = "grid";

//    }
//    else if ($('.viewmode-icon.list-view.active').length == 0) {
//        //remove selected from grid
//        $('.viewmode-icon.grid-view.active').removeClass("active");
//        //add to list
//        $('.viewmode-icon.list-view').addClass("active");
//        viewMode = "list";
//    }
//    SearchProductAjax();
//}
//jQuery(document).ready(function () {
//    currentCheckbox = $(this).attr("name");
//    $(".SpecsCheckbox").change(function () {
//        currentCheckbox = $(this).attr("name");
//        if ($(this).is(':checked')) {
//            enableCheckbox = true;
//        }
//        else {
//            enableCheckbox = false;
//        }
//        SearchProductAjax();
//    });

//    $(".ManuCheckbox").change(function () {
//        //currentCheckbox = $(this).attr("name");
//        //if ($(this).is( ":checked")) {
//        //    manufacturerId = $(this).val();
//        //    DisableManuCheckbox();
//        //    $(this).attr("disabled", false);
//        //    $(this).parent().removeClass("disabled");
//        //} else {
//        //    manufacturerId = "";
//        //    EnableManuCheckbox();
//        //}

//        //SearchProductAjax();
//        currentCheckbox = $(this).attr("name");
//        if ($(this).is(':checked')) {
//            enableCheckbox = true;
//        }
//        else {
//            enableCheckbox = false;
//        }
//        SearchProductAjax();
//    });
//});

//function DisableManuCheckbox() {
//    $(".ManuCheckbox").each(function( index ) {
//        $(this).attr("disabled", true);
//        $(this).parent().addClass("disabled");
//    });
//}
//function EnableManuCheckbox() {
//    $(".ManuCheckbox").each(function( index ) {
//        $(this).attr("disabled", false);
//        $(this).parent().removeClass("disabled");
//    });
//}

//function getParameterByName(name, url) {
//    name = name.replace(/[\[\]]/g, "\\$&");
//    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
//        results = regex.exec(url);
//    if (!results) return null;
//    if (!results[2]) return '';
//    return decodeURIComponent(results[2].replace(/\+/g, " "));
//}
//var specs = [];
//var attr = [];
//var minPrice = 0;
//var maxPrice = 10000000;
//var pagesize = 12;
//var pageNumber = 1;
//var viewMode = "grid";
//var sorting = "";
//var dataPost = {};
//var categoryId = "";
//var manufacturerIds = "";
//var term = "";
//var currentCheckbox;
//var enableCheckbox = true;
//var resetDropdown = false;
//var currentDropdown;
//var currentManufacturer;

//function SearchProductAjax(_pageNumber) {
//    pageNumber = _pageNumber;
//    if (pageNumber === undefined) {
//        pageNumber = 1;
//    }
//    specs = [];
//    var checkedSpecs = $('input[data-type="chk_spec_"]:checked').map(function () {
//        return parseInt(this.value);
//    }).get();
//    $.merge(specs, checkedSpecs);
    
//     manufacturerIds = $('input[data-type="manufacturer_chk"]:checked').map(function () {
//                    return parseInt($(this).val());
//                }).get().join(",");
//     console.log(manufacturerIds);
//    var dataPost = { specs: specs.join(","), attr: attr.join(","), categoryId: categoryId, viewmode: viewMode, pagesize: pagesize, pagenumber: pageNumber, orderby: sorting, manufacturerIds: manufacturerIds, pf: minPrice, pt: maxPrice };
//    addAntiForgeryToken(dataPost);
//    $(".loading-ajax").show();
//    $.ajax({
//        method: "GET",
//        url: "/Catalog/ProductFilterAjax",
//        data: dataPost
//    }).done(function (msg) {
        
//        if ($(".filter-ajax-result").length != 0) {
//            $(".filter-ajax-result").eq(0).html(msg.ViewData);
//        }
//        else {
//            $(".filter-ajax-result")[0].eq(0).html(msg.ViewData);
//        }
//        var checkedSpecSearch;
//        var checkedAttrSearch;
//        //loop all checkboxes and disable the checkboxes that cant be filtered

//        var checkedSpecs = $('input[data-type="chk_spec_"]').map(function () {

//            if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
//                if ($(this).attr("name") != currentCheckbox) {
//                    $(this).attr("disabled", true);
//                    $(this).parent().addClass("disabled");
//                    if ($(this).is(':checked')) {
//                        $(this).parent().removeClass("disabled");
//                        $(this).removeAttr("disabled");
//                    }
//                }
//            }
//            else {
//                $(this).parent().removeClass("disabled");
//                $(this).removeAttr("disabled");
//            }
//        });
//        ////loop all checkboxes and disable the checkboxes that cant be filtered
//        //var checkedSpecs = $('input[data-type="chk_attr_"]').map(function () {
//        //    if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
//        //        if ($(this).attr("name") != currentCheckbox) {
//        //            $(this).parent().addClass("disabled");
//        //        }
//        //    }
//        //    else {
//        //        $(this).parent().removeClass("disabled");

//        //    }
//        //});

//        var inputSpecSearch;
//        var inputAttrSearch;
//        if (resetDropdown) {
//            inputSpecSearch = 'select[data-type="cmb_spec_"] option';
//            inputAttrSearch = 'select[data-type="cmb_attr_"] option';
//        }
//        else {
//            inputSpecSearch = 'select[data-type="cmb_spec_"][name!="' + currentDropdown + '"] option';
//            inputAttrSearch = 'select[data-type="cmb_attr_"][name!="' + currentDropdown + '"] option';
//        }

//        //loop all input options and disable the checkboxes that cant be filtered
//        var inputSpecs = $(inputSpecSearch).map(function () {
//            if (this.value != 0) {
//                if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
//                    $(this).attr("disabled", true);
//                    $(this).parent().addClass("disabled");
//                }
//                else {
//                    $(this).parent().removeClass("disabled");
//                    $(this).removeAttr("disabled");
//                }
//            }
//        });
//        ////loop all input options and disable the checkboxes that cant be filtered
//        //var inputAttr = $(inputAttrSearch).map(function () {
//        //    if (this.value != 0) {
//        //        if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
//        //            $(this).attr("disabled", true);
//        //        }
//        //        else {
//        //            $(this).removeAttr("disabled");
//        //        }
//        //    }
//        //});

//        //        //filter out manufacturers
//                var manuFilter = $('input[data-type="manufacturer_chk"]').map(function () {
//                    if (this.value != 0) {
//                        if ($.inArray(parseInt(this.value), msg.ManufacturersIds) == -1) {
//                            $(this).attr("disabled", true);
//                            $(this).parent().addClass("disabled");
//                        }
//                        else {
//                            $(this).parent().removeClass("disabled");
//                            $(this).removeAttr("disabled");
//                        }
//                    }
//                });



//        enableCheckbox = true;
//        currentCheckbox = '';
//        currentDropdown = '';
//        currentManufacturer = '';
//        //resetDropdown = false;

//        $(".loading-ajax").hide();

//        if (history.pushState) {
//            var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '?specs=' + specs.join(",") + "&attr=" + attr.join(",") + "&categoryId=" + categoryId + "&viewmode=" + viewMode + "&pagesize=" + pagesize + "&pagenumber=" + pageNumber + "&manufacturerIds=" + manufacturerIds + "&orderby=" + sorting + "&pf=" + minPrice + "&pt=" + maxPrice;
//            window.history.pushState({ path: newurl }, '', newurl);
//        }

//        //$('.page-body > .pager').remove();
//        //spinner.spin(false);
//    })
//        .fail(function (error) {
//            $(".loading-ajax").hide();
//            console.log('error');
//            console.log(error);
//        });
//}