
    var attributes = {};
    var sliders = [];
    var text = [];
    var currentCheckbox;
    var enableCheckbox = true;
    var resetDropdown = false;
    var currentDropdown;
    var currentManufacturer;
    var priceSlider;

    if ('1' != '') {
        $.ajax({
            method: "GET",
            url: "/DebuggedWidgetFilter/CategoryHasProducts",
            dataType: "json",
            data: { 'categoryId': '1' }
        }).done(function (data) {
            if (data) {
                $(document).ready(function () {
                    if ($(".product-grid").length == 0 & $(".product-list").length == 0) {
                        //refresh page fallback
                        if ('1' != '') {
                            window.location = window.location.protocol + "//" + window.location.host + window.location.pathname;
                        }
                    }
                })
                DisplayPrice();
                GetAttributes();
            }
            else {
                $(".filter").hide();
            }
        });
    }
    else {
        DisplayPrice();
        GetAttributes();
    }

    function ClearSettings() {
        //reset all sliders to default
        for (var i = 0; i < sliders.length; i++) {
            var options = sliders[i].slider.noUiSlider.options;
            sliders[i].slider.noUiSlider.off('set');
            sliders[i].slider.noUiSlider.set([options.start[0], options.start[1]])
        }
        $(".DebuggedCheckbox").off('change');
        //reset all checkboxes to default
        $(".DebuggedCheckbox:checkbox:checked").each(function () {
            $(this).prop("checked", false);
        });

        //reset manufact
        $("#manufacturers").val(0);

        $(".DebuggedDropdown").each(function () {
            $(this).val(0);
        });

        var priceOptions = priceSlider.noUiSlider.options;
        priceSlider.noUiSlider.off('set');
        priceSlider.noUiSlider.set([priceOptions.range.min, priceOptions.range.max])

        searchParameters();
        for (var i = 0; i < sliders.length; i++) {
            sliders[i].slider.noUiSlider.on('set', function () {
                var postData = searchParameters();
            });
        }
        $(".DebuggedCheckbox").change(function () {
            currentCheckbox = $(this).attr("name");
            if ($(this).is(':checked')) {
                enableCheckbox = true;
            }
            else {
                enableCheckbox = false;
            }
            searchParameters();
        });
        priceSlider.noUiSlider.on('set', function () {
            var postData = searchParameters();
        });
    }

    function DisplayPrice() {
            
        priceSlider = document.getElementById('slider-handles_price');
        if ('1' != '') {
            $.ajax({
                method: "GET",
                url: "/DebuggedWidgetFilter/GetPrice",
                dataType: "json",
                data: { 'categoryId': '1' }
            }).done(function (data) {
                var minprice = '11';
                var maxprice = '17';
                if (minprice == '') {
                    minprice = data[0].MinPrice;
                }
                if (maxprice == '') {
                    maxprice = data[0].MaxPrice;
                }
                noUiSlider.create(priceSlider, {
                    start: [parseInt(minprice), parseInt(maxprice)],
                    connect: true,
                    step: 1,
                    range: {
                        'min': parseInt(data[0].MinPrice),
                        'max': parseInt(data[0].MaxPrice)
                    },
                    format: wNumb({
                        decimals: 0
                    })
                });

                //on update also update the range values
                priceSlider.noUiSlider.on('update', function (values, handle) {
                    if (handle == 1) {
                        document.getElementById('slider-snap-value-upper_price').innerHTML = values[handle];
                    }
                    else {
                        document.getElementById('slider-snap-value-lower_price').innerHTML = values[handle];
                    }
                });

                //insert slider in array object so we can access it after the initialisation
                //sliders.push({ slider: handlesSlider, name: obj[i][0].Name, id: obj[i][0].Attribute_Id });

                //on set: get all selected options and make the ajax call
                priceSlider.noUiSlider.on('set', function () {
                    var postData = searchParameters();
                });
            }).fail(function (error) {
                console.log('error');
                console.log(error);
            });
        }
        else {
            $.ajax({
                method: "GET",
                url: "/DebuggedWidgetFilter/GetSearchPrice",
                dataType: "json",
                data: { 'categoryId': $("#cid").val() }
            }).done(function (data) {
                var minprice = '11';
                var maxprice = '17';
                if (minprice == '') {
                    minprice = data.MinPrice;
                }
                if (maxprice == '') {
                    maxprice = data.MaxPrice;
                }
                noUiSlider.create(priceSlider, {
                    start: [parseInt(minprice), parseInt(maxprice)],
                    connect: true,
                    step: 1,
                    range: {
                        'min': parseInt(data.MinPrice),
                        'max': parseInt(data.MaxPrice)
                    },
                    format: wNumb({
                        decimals: 0
                    })
                });

                //on update also update the range values
                priceSlider.noUiSlider.on('update', function (values, handle) {
                    if (handle == 1) {
                        document.getElementById('slider-snap-value-upper_price').innerHTML = values[handle];
                    }
                    else {
                        document.getElementById('slider-snap-value-lower_price').innerHTML = values[handle];
                    }
                });

                //insert slider in array object so we can access it after the initialisation
                //sliders.push({ slider: handlesSlider, name: obj[i][0].Name, id: obj[i][0].Attribute_Id });

                //on set: get all selected options and make the ajax call
                priceSlider.noUiSlider.on('set', function () {
                    var postData = searchParameters();
                });
            }).fail(function (error) {
                console.log('error');
                console.log(error);
            });
        }
        
    }


    var opts = {
        lines: 13 // The number of lines to draw
        , length: 9 // The length of each line
        , width: 8 // The line thickness
        , radius: 34 // The radius of the inner circle
        , scale: 1 // Scales overall size of the spinner
        , corners: 0.8 // Corner roundness (0..1)
        , color: '#000' // #rgb or #rrggbb or array of colors
        , opacity: 0.15 // Opacity of the lines
        , rotate: 0 // The rotation offset
        , direction: 1 // 1: clockwise, -1: counterclockwise
        , speed: 1.3 // Rounds per second
        , trail: 56 // Afterglow percentage
        , fps: 20 // Frames per second when using setTimeout() as a fallback for CSS
        , zIndex: 2e9 // The z-index (defaults to 2000000000)
        , className: 'spinner' // The CSS class to assign to the spinner
        , top: '50%' // Top position relative to parent
        , left: '50%' // Left position relative to parent
        , shadow: false // Whether to render a shadow
        , hwaccel: false // Whether to use hardware acceleration
        , position: 'absolute' // Element positioning
    }
    var target = document.getElementsByClassName('center-2')
    var spinner = new Spinner(opts);
    if ('' == '') {
        $.ajax({
            method: "GET",
            url: "/DebuggedWidgetFilter/GetManufactures",
            dataType: "json",
        }).done(function (msg) {
            var manufacturerId = '';
            var obj = $.parseJSON(msg);
            if (obj.DisplayType == "Dropdown") {
                $.each(obj.Manufacturers, function (i, p) {
                    if (p.Id == manufacturerId) {
                        $('#manufacturers').append($('<option selected></option>').val(p.Id).html(p.Name));
                    }
                    else {
                        $('#manufacturers').append($('<option></option>').val(p.Id).html(p.Name));
                    }
                });
            }
            else {
                $.each(obj.Manufacturers, function (i, p) {
                    if (p.Id == manufacturerId) {
                        $('#manufacturers').append('<div class="checkbox"><input type="checkbox" data-type="manufacturer_chk" id="manufacturer_chk_' + p.Id + '" checked class="DebuggedCheckbox checkbox" name="manufacturer_chk" value="' + p.Id + '"><label for="manufacturer_chk_' + p.Id + '"><span>' + p.Name + '</span></label></div>');
                    }
                    else {
                        $('#manufacturers').append('<div class="checkbox"><input type="checkbox" class="DebuggedCheckbox checkbox" data-type="manufacturer_chk" id="manufacturer_chk_' + p.Id + '" name="manufacturer_chk" value="' + p.Id + '"><label for="manufacturer_chk_' + p.Id + '"><span>' + p.Name + '</span></label></div>');
                    }
                });
                $(".DebuggedCheckbox").change(function () {
                    currentCheckbox = $(this).attr("name");
                    if ($(this).is(':checked')) {
                        enableCheckbox = true;
                    }
                    else {
                        enableCheckbox = false;
                    }
                    searchParameters();
                });
            }

        })
        .fail(function (error) {
            console.log('error');
            console.log(error);
        });
    }
    function decimalPlaces(num) {
        var match = ('' + num).match(/(?:\.(\d+))?(?:[eE]([+-]?\d+))?$/);
        if (!match) { return 0; }
        return Math.max(
             0,
             // Number of digits right of decimal point.
             (match[1] ? match[1].length : 0)
             // Adjust for scientific notation.
             - (match[2] ? +match[2] : 0));
    }

    function getParameterByName(name, url) {
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    }

    function selectViewMode(type) {
        //if grid is selected and grid is not active
        if ($('.viewmode-icon.grid.selected').length == 0 & type == "Grid") {
            $('.viewmode-icon.list.selected').removeClass("selected");
            $('.viewmode-icon.grid').addClass("selected");

        }
        else if ($('.viewmode-icon.list.selected').length == 0 & type == "List") {
            //remove selected from grid
            $('.viewmode-icon.grid.selected').removeClass("selected");
            //add to list
            $('.viewmode-icon.list').addClass("selected");
        }
        searchParameters();
    }
    $(document).ready(function () {
        $("#products-orderby")[0].onchange = null;
        $("#products-pagesize")[0].onchange = null;
    });

    $(document).on('change', '#products-orderby, #products-pagesize', function (event) {
        event.preventDefault();
        searchParameters();
    });

    $(document).on('click', '.viewmode-icon', function (event) {
        event.preventDefault();
        selectViewMode($(this)[0].title);
    });

    $(document).on('click', '.individual-page a, .next-page a, .previous-page a', function (event) {
        event.preventDefault();
        searchParameters(getParameterByName("pagenumber", $(this)[0].href) == null ? 0 : getParameterByName("pagenumber", $(this)[0].href));
    });

    function searchParameters(pageNumber) {

        spinner.spin(target[0]);
        var start = new Date().getTime();

        var specs = [];
        var attr = [];

        //check all options for each slider that is configured
        for (var i = 0; i < sliders.length; i++) {
            var values = sliders[i].slider.noUiSlider.get();

            if (sliders[i].id.indexOf('txt_') != -1) {
                if (parseInt(values[0]) == 0 & parseInt(values[1]) == text[sliders[i].id].length - 1) {

                }
                else {
                    for (var ii = parseInt(values[0]) ; ii < parseInt(values[1]) + 1; ii++) {
                        if (sliders[i].id.indexOf('txt_spec') == -1) {
                            attr.push(text[sliders[i].id][ii].value);
                        }
                        else {
                            specs.push(text[sliders[i].id][ii].value);
                        }
                    }
                }
            }
            else {
                //get position of first
                if (sliders[i].prefix) {
                    values[0] = values[0].slice(sliders[i].prefix.length);
                    values[1] = values[1].slice(sliders[i].prefix.length);
                }

                if (sliders[i].postfix) {
                    values[0] = values[0].slice(0,-sliders[i].postfix.length);
                    values[1] = values[1].slice(0,-sliders[i].postfix.length);
                }

                var startRange = attributes[sliders[i].name].map(function (e) { return e.Value; }).indexOf(parseFloat(values[0]));

                //get position of second
                var endRange = attributes[sliders[i].name].map(function (e) { return e.Value; }).indexOf(parseFloat(values[1]));

                //if start and end are equal to max ranges dont add them.
                if (startRange == 0 & endRange == attributes[sliders[i].name].length - 1) {

                }
                else {
                    //add all items between this range
                    for (var ii = startRange; ii < endRange + 1; ii++) {
                        if (sliders[i].id.indexOf('attr') == -1) {
                            specs.push(attributes[sliders[i].name][ii].Id);
                        }
                        else {
                            attr.push(attributes[sliders[i].name][ii].Id);
                        }
                    }
                }
            }

        }
        var checkedAttr = $('input[data-type="chk_attr_"]:checked').map(function () {
            return parseInt(this.value);
        }).get();
        $.merge(attr, checkedAttr);

        var checkedSpecs = $('input[data-type="chk_spec_"]:checked').map(function () {
            return parseInt(this.value);
        }).get();
        $.merge(specs, checkedSpecs);

        var selectedAttr = $('select[data-type="cmb_attr_"]').map(function () {
            return parseInt($(this).find(":selected").val());
        }).get();
        $.merge(attr, selectedAttr);

        var selectedSpecs = $('select[data-type="cmb_spec_"]').map(function () {
            return parseInt($(this).find(":selected").val());
        }).get();
        $.merge(specs, selectedSpecs);
        var viewMode = "list";
        if ($('.viewmode-icon.grid.selected').length != 0) {
            viewMode = "grid";
        }

        if (pageNumber == undefined) {
            pageNumber = 0;
        }
            
        if (priceSlider.noUiSlider == undefined) {
            var priceValues = ['11', '17'];
        }
        else {
            var priceValues = priceSlider.noUiSlider.get();
        }
        
                    
                var manufacturerId = $('input[data-type="manufacturer_chk"]:checked').map(function () {
                    return parseInt($(this).val());
                }).get().join(",");
            
        console.log(manufacturerId);
        var sorting = getParameterByName('orderby', $("#products-orderby").val()) != null ? getParameterByName('orderby', $("#products-orderby").val()) : 0;
        var pagesize = getParameterByName('pagesize', $("#products-pagesize").val()) != null ? getParameterByName('pagesize', $("#products-pagesize").val()) : 0;
        if ('' != '') {
            var manufacturerId = '';
            var dataPost = { specs: specs.join(","), attr: attr.join(","), viewmode: viewMode, pagesize: pagesize, pagenumber: pageNumber, orderby: sorting, manufacturerId: manufacturerId  , minPrice: priceValues[0], maxPrice: priceValues[1] };
            $.ajax({
                method: "GET",
                url: "/DebuggedWidgetFilter/AjaxManufacturer",
                data: dataPost
            }).done(function (msg) {
                if ($(".product-grid").length != 0) {
                    $(".product-grid").eq(0).html(msg.View);
                }
                else {
                    $(".product-list")[0].eq(0).html(msg.View);
                }
                var checkedSpecSearch;
                var checkedAttrSearch;
                //loop all checkboxes and disable the checkboxes that cant be filtered

                var checkedSpecs = $('input[data-type="chk_spec_"]').map(function () {

                    if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
                        if ($(this).attr("name") != currentCheckbox) {
                            $(this).attr("disabled", true);
                            $(this).parent().addClass("disabled");
                        }

                    }
                    else {
                        $(this).parent().removeClass("disabled");
                        $(this).removeAttr("disabled");
                    }
                });
                //loop all checkboxes and disable the checkboxes that cant be filtered
                var checkedSpecs = $('input[data-type="chk_attr_"]').map(function () {
                    if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
                        if ($(this).attr("name") != currentCheckbox) {
                            $(this).parent().addClass("disabled");
                        }
                    }
                    else {
                        $(this).parent().removeClass("disabled");

                    }
                });
                var inputSpecSearch;
                var inputAttrSearch;
                if (resetDropdown) {
                    inputSpecSearch = 'select[data-type="cmb_spec_"] option';
                    inputAttrSearch = 'select[data-type="cmb_attr_"] option';
                }
                else {
                    inputSpecSearch = 'select[data-type="cmb_spec_"][name!="' + currentDropdown + '"] option';
                    inputAttrSearch = 'select[data-type="cmb_attr_"][name!="' + currentDropdown + '"] option';
                }
                //loop all input options and disable the checkboxes that cant be filtered
                var inputSpecs = $(inputSpecSearch).map(function () {
                    if (this.value != 0) {
                        if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
                            $(this).attr("disabled", true);
                            $(this).parent().addClass("disabled");
                        }
                        else {
                            $(this).parent().removeClass("disabled");
                            $(this).removeAttr("disabled");
                        }
                    }
                });
                //loop all input options and disable the checkboxes that cant be filtered
                var inputAttr = $(inputAttrSearch).map(function () {
                    if (this.value != 0) {
                        if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
                            $(this).attr("disabled", true);
                        }
                        else {
                            $(this).removeAttr("disabled");
                        }
                    }
                });
                    
                        //filter out manufacturers
                        var manuFilter = $('input[data-type="manufacturer_chk"]').map(function () {
                            if (this.value != 0) {
                                if ($.inArray(parseInt(this.value), msg.ManufacturersIds) == -1) {
                                    $(this).attr("disabled", true);
                                    $(this).parent().addClass("disabled");
                                }
                                else {
                                    $(this).parent().removeClass("disabled");
                                    $(this).removeAttr("disabled");
                                }
                            }
                        });
                    


                enableCheckbox = true;
                currentCheckbox = '';
                currentDropdown = '';
                currentManufacturer = '';
                resetDropdown = false;
                if (history.pushState) {
                    var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '?specs=' + specs.join(",") + "&attr=" + attr.join(",") + "&categoryId=" + categoryId + "&viewmode=" + viewMode + "&pagesize=" + pagesize + "&pagenumber=" + pageNumber + "&manufacturerId=" + manufacturerId + "&orderby=" + sorting  + "&minPrice=" + priceValues[0] + "&maxPrice=" + priceValues[1] ;

                    window.history.pushState({ path: newurl }, '', newurl);
                }

                $('.page-body > .pager').remove();
                spinner.spin(false);
            })
            .fail(function (error) {
                console.log('error');
                console.log(error);
            });
        }
        else if ('' != '') {
            var tagId = '';
            var dataPost = { specs: specs.join(","), attr: attr.join(","), productTagId: tagId, viewmode: viewMode, pagesize: pagesize, pagenumber: pageNumber, orderby: sorting, manufacturerId: manufacturerId  , minPrice: priceValues[0], maxPrice: priceValues[1] };
            $.ajax({
                method: "GET",
                url: "/DebuggedWidgetFilter/AjaxTag",
                data: dataPost
            }).done(function (msg) {
                if ($(".product-grid").length != 0) {
                    $(".product-grid").eq(0).html(msg.View);
                }
                else {
                    $(".product-list")[0].eq(0).html(msg.View);
                }
                var checkedSpecSearch;
                var checkedAttrSearch;
                //loop all checkboxes and disable the checkboxes that cant be filtered

                var checkedSpecs = $('input[data-type="chk_spec_"]').map(function () {

                    if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
                        if ($(this).attr("name") != currentCheckbox) {
                            $(this).attr("disabled", true);
                            $(this).parent().addClass("disabled");
                        }

                    }
                    else {
                        $(this).parent().removeClass("disabled");
                        $(this).removeAttr("disabled");
                    }
                });
                //loop all checkboxes and disable the checkboxes that cant be filtered
                var checkedSpecs = $('input[data-type="chk_attr_"]').map(function () {
                    if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
                        if ($(this).attr("name") != currentCheckbox) {
                            $(this).parent().addClass("disabled");
                        }
                    }
                    else {
                        $(this).parent().removeClass("disabled");

                    }
                });
                var inputSpecSearch;
                var inputAttrSearch;
                if (resetDropdown) {
                    inputSpecSearch = 'select[data-type="cmb_spec_"] option';
                    inputAttrSearch = 'select[data-type="cmb_attr_"] option';
                }
                else {
                    inputSpecSearch = 'select[data-type="cmb_spec_"][name!="' + currentDropdown + '"] option';
                    inputAttrSearch = 'select[data-type="cmb_attr_"][name!="' + currentDropdown + '"] option';
                }
                //loop all input options and disable the checkboxes that cant be filtered
                var inputSpecs = $(inputSpecSearch).map(function () {
                    if (this.value != 0) {
                        if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
                            $(this).attr("disabled", true);
                            $(this).parent().addClass("disabled");
                        }
                        else {
                            $(this).parent().removeClass("disabled");
                            $(this).removeAttr("disabled");
                        }
                    }
                });
                //loop all input options and disable the checkboxes that cant be filtered
                var inputAttr = $(inputAttrSearch).map(function () {
                    if (this.value != 0) {
                        if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
                            $(this).attr("disabled", true);
                        }
                        else {
                            $(this).removeAttr("disabled");
                        }
                    }
                });
                    
                        //filter out manufacturers
                        var manuFilter = $('input[data-type="manufacturer_chk"]').map(function () {
                            if (this.value != 0) {
                                if ($.inArray(parseInt(this.value), msg.ManufacturersIds) == -1) {
                                    $(this).attr("disabled", true);
                                    $(this).parent().addClass("disabled");
                                }
                                else {
                                    $(this).parent().removeClass("disabled");
                                    $(this).removeAttr("disabled");
                                }
                            }
                        });
                    


                enableCheckbox = true;
                currentCheckbox = '';
                currentDropdown = '';
                currentManufacturer = '';
                resetDropdown = false;
                if (history.pushState) {
                    var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '?specs=' + specs.join(",") + "&attr=" + attr.join(",") + "&categoryId=" + categoryId + "&viewmode=" + viewMode + "&pagesize=" + pagesize + "&pagenumber=" + pageNumber + "&manufacturerId=" + manufacturerId + "&orderby=" + sorting   + "&minPrice=" + priceValues[0] + "&maxPrice=" + priceValues[1];
                    window.history.pushState({ path: newurl }, '', newurl);
                }

                $('.page-body > .pager').remove();
                spinner.spin(false);
            })
            .fail(function (error) {
                console.log('error');
                console.log(error);
            });

        }
        else if ('1' == '') {
            var dataPost = { q: $("#q").val(), adv: $("#adv").val(), adv: $("#adv").val(), cid: $("#cid").val(), isc: $("#isc").val(), mid: $("#mid").val(), sid: $("#sid").val(), manufacturerId: manufacturerId ,specs: specs.join(","), attr: attr.join(","), viewmode: viewMode, pagesize: pagesize, pagenumber: pageNumber, orderby: sorting  , pf: priceValues[0], pt: priceValues[1] };
            $.ajax({
                method: "GET",
                url: "/DebuggedWidgetFilter/AjaxSearch",
                data: dataPost
            }).done(function (msg) {
                if ($(".product-grid").length != 0) {
                    $(".product-grid").eq(0).html(msg.View);
                }
                else if ($(".product-list").length != 0) {
                    $(".product-list")[0].eq(0).html(msg.View);
                }
                $('.page-body > .pager').remove();

                var checkedSpecs = $('input[data-type="chk_spec_"]').map(function () {

                    if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
                        if ($(this).attr("name") != currentCheckbox) {
                            $(this).attr("disabled", true);
                            $(this).parent().addClass("disabled");
                        }

                    }
                    else {
                        $(this).parent().removeClass("disabled");
                        $(this).removeAttr("disabled");
                    }
                });
                //loop all checkboxes and disable the checkboxes that cant be filtered
                var checkedSpecs = $('input[data-type="chk_attr_"]').map(function () {
                    if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
                        if ($(this).attr("name") != currentCheckbox) {
                            $(this).parent().addClass("disabled");
                        }
                    }
                    else {
                        $(this).parent().removeClass("disabled");

                    }
                });
                var inputSpecSearch;
                var inputAttrSearch;
                if (resetDropdown) {
                    inputSpecSearch = 'select[data-type="cmb_spec_"] option';
                    inputAttrSearch = 'select[data-type="cmb_attr_"] option';
                }
                else {
                    inputSpecSearch = 'select[data-type="cmb_spec_"][name!="' + currentDropdown + '"] option';
                    inputAttrSearch = 'select[data-type="cmb_attr_"][name!="' + currentDropdown + '"] option';
                }
                //loop all input options and disable the checkboxes that cant be filtered
                var inputSpecs = $(inputSpecSearch).map(function () {
                    if (this.value != 0) {
                        if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
                            $(this).attr("disabled", true);
                            $(this).parent().addClass("disabled");
                        }
                        else {
                            $(this).parent().removeClass("disabled");
                            $(this).removeAttr("disabled");
                        }
                    }
                });

                //loop all input options and disable the checkboxes that cant be filtered
                var inputAttr = $(inputAttrSearch).map(function () {
                    if (this.value != 0) {
                        if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
                            $(this).attr("disabled", true);
                        }
                        else {
                            $(this).removeAttr("disabled");
                        }
                    }
                });
                    
                        //filter out manufacturers
                        var manuFilter = $('input[data-type="manufacturer_chk"]').map(function () {
                            if (this.value != 0) {
                                if ($.inArray(parseInt(this.value), msg.ManufacturersIds) == -1) {
                                    $(this).attr("disabled", true);
                                    $(this).parent().addClass("disabled");
                                }
                                else {
                                    $(this).parent().removeClass("disabled");
                                    $(this).removeAttr("disabled");
                                }
                            }
                        });
                    

                enableCheckbox = true;
                currentCheckbox = '';
                currentDropdown = '';
                currentManufacturer = '';
                resetDropdown = false;
                spinner.spin(false);
            })
            .fail(function (error) {
                console.log('error');
                console.log(error);
            });
        }
        else {
            var categoryId = '1';
            var dataPost = { specs: specs.join(","), attr: attr.join(","), categoryId: categoryId, viewmode: viewMode, pagesize: pagesize, pagenumber: pageNumber, orderby: sorting, manufacturerId: manufacturerId  , minPrice: priceValues[0], maxPrice: priceValues[1] };
            $.ajax({
                method: "GET",
                url: "/DebuggedWidgetFilter/AjaxCategory",
                data: dataPost
            }).done(function (msg) {
                if ($(".product-grid").length != 0) {
                    $(".product-grid").eq(0).html(msg.View);
                }
                else {
                    $(".product-list")[0].eq(0).html(msg.View);
                }
                var checkedSpecSearch;
                var checkedAttrSearch;
                //loop all checkboxes and disable the checkboxes that cant be filtered

                var checkedSpecs = $('input[data-type="chk_spec_"]').map(function () {

                    if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
                        if ($(this).attr("name") != currentCheckbox) {
                            $(this).attr("disabled", true);
                            $(this).parent().addClass("disabled");
                        }

                    }
                    else {
                        $(this).parent().removeClass("disabled");
                        $(this).removeAttr("disabled");
                    }
                });
                //loop all checkboxes and disable the checkboxes that cant be filtered
                var checkedSpecs = $('input[data-type="chk_attr_"]').map(function () {
                    if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
                        if ($(this).attr("name") != currentCheckbox) {
                            $(this).parent().addClass("disabled");
                        }
                    }
                    else {
                        $(this).parent().removeClass("disabled");

                    }
                });
                var inputSpecSearch;
                var inputAttrSearch;
                if (resetDropdown) {
                    inputSpecSearch = 'select[data-type="cmb_spec_"] option';
                    inputAttrSearch = 'select[data-type="cmb_attr_"] option';
                }
                else {
                    inputSpecSearch = 'select[data-type="cmb_spec_"][name!="' + currentDropdown + '"] option';
                    inputAttrSearch = 'select[data-type="cmb_attr_"][name!="' + currentDropdown + '"] option';
                }
                //loop all input options and disable the checkboxes that cant be filtered
                var inputSpecs = $(inputSpecSearch).map(function () {
                    if (this.value != 0) {
                        if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
                            $(this).attr("disabled", true);
                            $(this).parent().addClass("disabled");
                        }
                        else {
                            $(this).parent().removeClass("disabled");
                            $(this).removeAttr("disabled");
                        }
                    }
                });
                //loop all input options and disable the checkboxes that cant be filtered
                var inputAttr = $(inputAttrSearch).map(function () {
                    if (this.value != 0) {
                        if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
                            $(this).attr("disabled", true);
                        }
                        else {
                            $(this).removeAttr("disabled");
                        }
                    }
                });
                    
                        //filter out manufacturers
                        var manuFilter = $('input[data-type="manufacturer_chk"]').map(function () {
                            if (this.value != 0) {
                                if ($.inArray(parseInt(this.value), msg.ManufacturersIds) == -1) {
                                    $(this).attr("disabled", true);
                                    $(this).parent().addClass("disabled");
                                }
                                else {
                                    $(this).parent().removeClass("disabled");
                                    $(this).removeAttr("disabled");
                                }
                            }
                        });
                    


                enableCheckbox = true;
                currentCheckbox = '';
                currentDropdown = '';
                currentManufacturer = '';
                resetDropdown = false;
                if (history.pushState) {
                    var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '?specs=' + specs.join(",") + "&attr=" + attr.join(",") + "&categoryId=" + categoryId + "&viewmode=" + viewMode + "&pagesize=" + pagesize + "&pagenumber=" + pageNumber + "&manufacturerId=" + manufacturerId + "&orderby=" + sorting   + "&minPrice=" + priceValues[0] + "&maxPrice=" + priceValues[1];
                    window.history.pushState({ path: newurl }, '', newurl);
                }

                $('.page-body > .pager').remove();
                spinner.spin(false);
            })
            .fail(function (error) {
                console.log('error');
                console.log(error);
            });
        }
    }
    function GetAttributes() {
        $.ajax({
            method: "GET",
            url: "/DebuggedWidgetFilter/GetAttributes?CategoryId=1",
            dataType: "json",
        }).done(function (msg) {
            var specs = '5,36,0';
            var attr = '';
            specs = specs.split(",");
            attr = attr.split(",");

            var obj = $.parseJSON(msg);
            var currentGroup = Number.NEGATIVE_INFINITY;
            //loop over all attributes and convert them to a convient object
            for (var i = 0; i < obj.length; i++) {

                if (obj[i].length <= 1 & (obj[i][0].Type == "Slider with numeric steps" | obj[i][0].Type == "Slider with ranges" | obj[i][0].Type == "Slider with textual steps (multi-language support)")) {
                    continue;
                }
                //new group so create new div
                if (currentGroup != obj[i][0].Group) {
                    var groupHtml = '<div id="group_' + obj[i][0].Group + '" class="filter-group"></div>';
                    $("#container").append(groupHtml);
                    currentGroup = obj[i][0].Group;
                }
                if (obj[i][0].Type == "Multiple checkboxes") {
                    var html = '<h3>' + obj[i][0].Name + '</h3>';
                    var prefix = "chk_" + (obj[i][0].Attribute_Id == null ? "spec_" : "attr_");
                    var id = obj[i][0].Attribute_Id == null ? obj[i][0].SpecificationAttribute_Id : obj[i][0].Attribute_Id;

                    for (var ii = 0; ii < obj[i].length; ii++) {
                        //check if value is set in querystring
                        if (prefix == "chk_spec_") {
                            var checked = $.inArray(obj[i][ii].Id + "", specs) != -1 ? "checked" : "";
                        }
                        else {
                            var checked = $.inArray(obj[i][ii].Id + "", attr) != -1 ? "checked" : "";
                        }
                        html += '<div class="checkbox">' +
                                    '<input type="checkbox" data-type="' + prefix + '" id="' + prefix + obj[i][ii].Id + '" class="DebuggedCheckbox checkbox"' + checked + ' name="' + id + '" value="' + obj[i][ii].Id + '" />' +
                                    '<label for="' + prefix + obj[i][ii].Id + '">';


                        if (obj[i][ii].ColorSquaresRgb != null) {
                            html += '<div class="attribute-square" style="background-color:' + obj[i][ii].ColorSquaresRgb + '"></div>';
                        }

                        html += '<span>' + obj[i][ii].Value + '</span>' +
                               '</label>'+ '</div>';
                    }
                    $("#group_" + currentGroup).append(html);
                }
                else if (obj[i][0].Type == "Single checkbox") {
                    if (!$('#characteristic_' + obj[i][0].Characteristic.replace(" ","_"), "#group_" + currentGroup).length) {
                        var charHtml = '<div id="characteristic_' + obj[i][0].Characteristic.replace(" ", "_") + '">' +
                                            '<h3>' + obj[i][0].Characteristic + '</h3>' +
                                       '</div>';
                        $("#group_" + currentGroup).append(charHtml);
                    }
                    var html = '';
                    var prefix = "chk_" + (obj[i][0].Attribute_Id == null ? "spec_" : "attr_");
                    var id = obj[i][0].Attribute_Id == null ? obj[i][0].SpecificationAttribute_Id : obj[i][0].Attribute_Id;

                    for (var ii = 0; ii < obj[i].length; ii++) {
                        if (prefix == "chk_spec_") {
                            var checked = $.inArray(obj[i][ii].Id + "", specs) != -1 ? "checked" : "";
                        }
                        else {
                            var checked = $.inArray(obj[i][ii].Id + "", attr) != -1 ? "checked" : "";
                        }
                        html += '<div class="checkbox">' +
                                    '<input type="checkbox" data-type="' + prefix + '" id="' + prefix + obj[i][ii].Id + '" class="DebuggedCheckbox checkbox"' + checked + ' name="' + id + '" value="' + obj[i][ii].Id + '" />' +
                                    '<label for="' + prefix + obj[i][ii].Id + '">';


                        if (obj[i][ii].ColorSquaresRgb != null) {
                            html += '<div class="attribute-square" style="background-color:' + obj[i][ii].ColorSquaresRgb + '"></div>';
                        }

                        html += '<span>' + obj[i][ii].Value + '</span>' +
                               '</label>' + '</div>';
                    }
                    $('#characteristic_' + obj[i][0].Characteristic.replace(" ", "_"), "#group_" + currentGroup).append(html);
                }
                else if (obj[i][0].Type == "Dropdown") {
                    var prefix = "cmb_" + (obj[i][0].Attribute_Id == null ? "spec_" : "attr_");
                    var id = obj[i][0].Attribute_Id == null ? obj[i][0].SpecificationAttribute_Id : obj[i][0].Attribute_Id;
                    var html = '<h3>' + obj[i][0].Name + '</h3>';
                    html += '<select id="' + prefix + id + '" class="DebuggedDropdown" name="' + id + '" data-type="' + prefix + '" onchange="searchParameters()">';
                    html += '<option value="0">---</option>'
                    for (var ii = 0; ii < obj[i].length; ii++) {
                        //check if value is set in querystring
                        if (prefix == "cmb_spec_") {
                            var selected = $.inArray(obj[i][ii].Id + "", specs) != -1 ? "selected=\"selected\"" : "";
                        }
                        else {
                            var selected = $.inArray(obj[i][ii].Id + "", attr) != -1 ? "selected=\"selected\"" : "";
                        }
                        html += '<option value="' + obj[i][ii].Id + '" ' + selected + '>' + obj[i][ii].Value + '</option>';                      
                    }
                    html += '</select>'
                    $("#group_" + currentGroup).append(html);
                }
                else if (obj[i][0].Type == "Slider with ranges" | obj[i][0].Type == "Slider with numeric steps") {
                    var lowest = Number.POSITIVE_INFINITY;
                    var highest = Number.NEGATIVE_INFINITY;
                    var lowestSpec = Number.POSITIVE_INFINITY;
                    var highestSpec = Number.NEGATIVE_INFINITY;
                    var tmp;
                    var range = {};
                    attributes[obj[i][0].Name] = [];
                    obj[i].sort(function (a, b) {
                        // a and b will here be two objects from the array
                        // thus a[1] and b[1] will equal the names
                        var x = parseFloat(a.Value.replace(',', '.'));
                        var y = parseFloat(b.Value.replace(',', '.'));
                        // if they are equal, return 0 (no sorting)
                        if (x == y) { return 0; }
                        if (x > y) {
                            // if a should come after b, return 1
                            return 1;
                        }
                        else {
                            // if b should come after a, return -1
                            return -1;
                        }
                    });
                    for (var ii = 0; ii < obj[i].length; ii++) {
                        tmp = parseFloat(obj[i][ii].Value.replace(',', '.'));
                        if (specs.length != 0 | attr.length != 0) {
                            if (obj[i][ii].Attribute_Id == null) {
                                if (tmp < lowestSpec & $.inArray(obj[i][ii].Id + "", specs) != -1) lowestSpec = tmp;
                                if (tmp > highestSpec & $.inArray(obj[i][ii].Id + "", specs) != -1) highestSpec = tmp;
                            }
                            else {
                                if (tmp < lowestSpec & $.inArray(obj[i][ii].Id + "", attr) != -1) lowestSpec = tmp;
                                if (tmp > highestSpec & $.inArray(obj[i][ii].Id + "", attr) != -1) highestSpec = tmp;
                            }

                        }
                        if (tmp < lowest) lowest = tmp;
                        if (tmp > highest) highest = tmp;

                        var percile = 100 / (obj[i].length - 1);
                        var percent = percile * ii;
                        range[percent] = parseFloat(obj[i][ii].Value.replace(',', '.'));
                        attributes[obj[i][0].Name].push({ Id: obj[i][ii].Id, Value: parseFloat(obj[i][ii].Value.replace(',', '.')) });
                    }
                    range['min'] = parseFloat(obj[i][0].Value.replace(',', '.'));
                    range['max'] = parseFloat(obj[i][obj[i].length - 1].Value.replace(',', '.'));

                    //add slider html to frontend
                    var sliderId = obj[i][0].Attribute_Id == null ? "spec_" + obj[i][0].SpecificationAttribute_Id : "attr_" + obj[i][0].Attribute_Id;
                    var html = '<h3>' + obj[i][0].Name + '</h3>';
                    html += '<div data-id="' + sliderId + '" id="slider-handles_' + sliderId + '"></div>';
                    html += '<div class="slider-values">' +
                                '<span class="slider-value-lower" id="slider-snap-value-lower_' + sliderId + '"></span>' +
                                '<span class="slider-value-upper" id="slider-snap-value-upper_' + sliderId + '"></span>' +
                            '</div>';
                    $("#group_" + currentGroup).append(html);

                    var handlesSlider = document.getElementById('slider-handles_' + sliderId);

                    if (obj[i][0].Type == "Slider with numeric steps") {
                        noUiSlider.create(handlesSlider, {
                            start: [lowest, highest],
                            connect: true,
                            step: obj[i][0].Steps,
                            range: {
                                'min': lowest,
                                'max': highest
                            },
                            format: wNumb({
                                decimals: decimalPlaces(obj[i][0].Steps),
                                postfix: obj[i][0].Postfix != null ? obj[i][0].Postfix : "",
                                prefix: obj[i][0].Prefix != null ? obj[i][0].Prefix : ""
                            })
                        });
                    }
                    else {
                        noUiSlider.create(handlesSlider, {
                            start: [
                                lowest,
                                highest
                            ],
                            range: range,
                            connect: true,
                            snap: true,
                            format: wNumb({
                                decimals: 0,
                                postfix: obj[i][0].Postfix != null ? obj[i][0].Postfix : "",
                                prefix: obj[i][0].Prefix != null ? obj[i][0].Prefix : ""
                            })
                        });
                    }

                    if (lowestSpec != Number.POSITIVE_INFINITY & highestSpec != Number.NEGATIVE_INFINITY) {
                        handlesSlider.noUiSlider.set([lowestSpec, highestSpec]);
                    }


                    handlesSlider.noUiSlider.on('update', function (values, handle) {
                        if (handle == 1) {
                            document.getElementById('slider-snap-value-upper_' + $(this.target).data("id")).innerHTML = values[handle];
                        }
                        else {
                            document.getElementById('slider-snap-value-lower_' + $(this.target).data("id")).innerHTML = values[handle];
                        }
                    });

                    //insert slider in array object so we can access it after the initialisation
                    sliders.push({ slider: handlesSlider, name: obj[i][0].Name, id: sliderId, prefix: obj[i][0].Prefix, postfix: obj[i][0].Postfix });

                    //on set: get all selected options and make the ajax call
                    handlesSlider.noUiSlider.on('set', function () {
                        var postData = searchParameters();
                    });
                }
                else if (obj[i][0].Type == "Slider with textual steps (multi-language support)") {
                    var lowestSpec = Number.POSITIVE_INFINITY;
                    var highestSpec = Number.NEGATIVE_INFINITY;
                    var tmp;
                    var sliderId = obj[i][0].Attribute_Id == null ? "txt_spec_" + obj[i][0].SpecificationAttribute_Id : "txt_attr_" + obj[i][0].Attribute_Id;
                    text[sliderId] = [];
                    for (var ii = 0; ii < obj[i].length; ii++) {
                        if (specs.length != 0 | attr.length != 0) {
                            if (obj[i][ii].Attribute_Id == null) {
                                if (tmp < lowestSpec & $.inArray(obj[i][ii].Id + "", specs) != -1) lowestSpec = ii;
                                if (tmp > highestSpec & $.inArray(obj[i][ii].Id + "", specs) != -1) highestSpec = ii;
                            }
                            else {
                                if (tmp < lowestSpec & $.inArray(obj[i][ii].Id + "", attr) != -1) lowestSpec = ii;
                                if (tmp > highestSpec & $.inArray(obj[i][ii].Id + "", attr) != -1) highestSpec = ii;
                            }

                        }
                        if (tmp < lowest) lowest = tmp;
                        if (tmp > highest) highest = tmp;
                        text[sliderId].push({ value: obj[i][ii].Id, name: obj[i][ii].Value });
                    }

                    //add slider html to frontend
                    var html = '<h3>' + obj[i][0].Name + '</h3>';
                    html += '<div data-id="' + sliderId + '" id="slider-handles_' + sliderId + '"></div>';
                    html += '<div class="slider-values">' +
                                '<span class="slider-value-lower" id="slider-snap-value-lower_' + sliderId + '"></span>' +
                                '<span class="slider-value-upper" id="slider-snap-value-upper_' + sliderId + '"></span>' +
                            '</div>';
                    $("#group_" + currentGroup).append(html);

                    var handlesSlider = document.getElementById('slider-handles_' + sliderId);

                    noUiSlider.create(handlesSlider, {
                        start: [0, obj[i].length - 1],
                        connect: true,
                        step: 1,
                        range: {
                            'min': 0,
                            'max': obj[i].length - 1
                        },
                        format: wNumb({
                            postfix: obj[i][0].Postfix != null ? obj[i][0].Postfix : "",
                            prefix: obj[i][0].Prefix != null ? obj[i][0].Prefix : ""
                        })
                    });

                    //on update also update the range values
                    handlesSlider.noUiSlider.on('update', function (values, handle) {
                        if (handle == 1) {
                            document.getElementById('slider-snap-value-upper_' + $(this.target).data("id")).innerHTML = text[$(this.target).data("id")][values[handle]].name;
                        }
                        else {
                            document.getElementById('slider-snap-value-lower_' + $(this.target).data("id")).innerHTML = text[$(this.target).data("id")][values[handle]].name;
                        }
                    });

                    if (lowestSpec != Number.POSITIVE_INFINITY & highestSpec != Number.NEGATIVE_INFINITY) {
                        handlesSlider.noUiSlider.set([lowestSpec, highestSpec]);
                    }

                    //insert slider in array object so we can access it after the initialisation
                    sliders.push({ slider: handlesSlider, name: obj[i][0].Name, id: sliderId, prefix: obj[i][0].Prefix, postfix: obj[i][0].Postfix });

                    //on set: get all selected options and make the ajax call
                    handlesSlider.noUiSlider.on('set', function () {
                        var postData = searchParameters();
                    });

                }
            }
            $(".DebuggedCheckbox").change(function () {
                currentCheckbox = $(this).attr("name");
                if ($(this).is(':checked')) {
                    enableCheckbox = true;
                }
                else {
                    enableCheckbox = false;
                }
                searchParameters();
            });

            $(".DebuggedDropdown").change(function () {
                currentDropdown = $(this).attr("name");
                if ($(this).val() == 0) {
                    resetDropdown = true;
                }
                else {
                    resetDropdown = false;
                }
                searchParameters();
            });

            $("#manufacturers").change(function () {
                currentManufacturer = $(this).val();
            });
            searchParameters();
        })
             .fail(function (error) {
                 console.log('error');
                 console.log(error);
             });
    }

