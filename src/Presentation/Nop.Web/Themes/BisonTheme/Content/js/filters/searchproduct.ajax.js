

var attributes = {};
var sliders = [];
var text = [];
var currentCheckbox;
var enableCheckbox = true;
var resetDropdown = false;
var currentDropdown;
var currentManufacturer;
var priceSlider;
var categoryId = '';
var manufacturerIds = '';
var specs = [];
var viewMode = "grid";

if (categoryId != '') {
    $.ajax({
        method: "GET",
        url: "/Catalog/CategoryHasProducts",
        dataType: "json",
        data: { 'categoryId': categoryId }
    }).done(function (data) {
        if (data) {
            $(document).ready(function () {
                if ($(".product-grid").length == 0 & $(".product-list").length == 0) {
                    //refresh page fallback
                    if (categoryId != '') {
                        window.location = window.location.protocol +
                            "//" +
                            window.location.host +
                            window.location.pathname;
                    }
                }
            });
            DisplayPrice();
            //GetAttributes();
        }
        else {
            $(".filter").hide();
        }
    });
}
else {
    DisplayPrice();
    //GetAttributes();
}

function ClearSettings() {
    //reset all sliders to default
    for (var i = 0; i < sliders.length; i++) {
        var options = sliders[i].slider.noUiSlider.options;
        sliders[i].slider.noUiSlider.off('set');
        sliders[i].slider.noUiSlider.set([options.start[0], options.start[1]]);
    }
    $(".SpecCheckbox,.CateCheckbox,.ManufactureCheckbox").off('change');

    //reset all checkboxes to default
    $(".SpecCheckbox:checkbox:checked").each(function () {
        $(this).prop("checked", false);
    });

    $(".CateCheckbox:checkbox:checked").each(function () {
        $(this).prop("checked", false);
    });

    $(".ManufactureCheckbox:checkbox:checked").each(function () {
        $(this).prop("checked", false);
    });

    if ($("#q").val() != undefined) {
        $("#q").val('');
    }
    //reset manufact
    $("#manufacturers").val(0);

    $("#categories").val(0);
    

    var priceOptions = priceSlider.noUiSlider.options;
    priceSlider.noUiSlider.off('set');
    priceSlider.noUiSlider.set([priceOptions.range.min, priceOptions.range.max]);

    searchParameters();
    for (var i = 0; i < sliders.length; i++) {
        sliders[i].slider.noUiSlider.on('set', function () {
            var postData = searchParameters();
        });
    }
    $(".SpecCheckbox,.CateCheckbox,.ManufactureCheckbox").change(function () {
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
    if (priceSlider != null && priceSlider != undefined) {
        if (categoryId == '' && $('#cid').val() != undefined) {
            categoryId = $('#cid').val();
        }
        $.ajax({
            method: "GET",
            url: "/Catalog/GetPrice",
            dataType: "json",
            data: { 'categoryId': categoryId }
        }).done(function (data) {
            var minprice = data[0].MinPrice;
            var maxprice = data[0].MaxPrice;
            console.log(priceSlider);
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
                    //            $(".filter-from-price").val(ui.values[0].format());
                    //            $(".filter-to-price").val(ui.values[1].format());
                    $('.filter-to-price').val(values[handle].toInt().format());

                    $('.price-to').val(values[handle].toInt());
                }
                else {
                    $('.filter-from-price').val(values[handle].toInt().format());
                    $('.price-from').val(values[handle].toInt());
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
var target = document.getElementsByClassName('center-2');
var spinner = new Spinner(opts);

if (''  == '') {
    var parentCateId = 0;
    if ($('#cid').val() != undefined && $('#cid').val() !== '') {
        parentCateId = $('#cid').val();
    }
    $.ajax({
        method: "GET",
        url: "/Catalog/GetSubCategories",
        dataType: "json",
        data: { 'categoryId': parentCateId }
    }).done(function (msg) {
        var obj = $.parseJSON(JSON.stringify(msg));
        $.each(obj.Categories, function (i, p) {
            $('#categories').append('<div class="checkbox"><input type="checkbox" class="CateCheckbox checkbox" data-type="chk_cate_" id="chk_cate_' + p.Id + '" name="chk_cate" value="' + p.Id + '"><label for="chk_cate_' + p.Id + '"><span>' + p.Name + '</span></label></div>');
        });
        $(".CateCheckbox").change(function () {
            currentCheckbox = $(this).attr("name");
            if ($(this).is(':checked')) {
                enableCheckbox = true;
            }
            else {
                enableCheckbox = false;
            }
            searchParameters();
        });

    })
        .fail(function (error) {
            console.log('error');
            console.log(error);
        });
}

if (manufacturerIds == '') {
    $.ajax({
        method: "GET",
        url: "/Catalog/GetManufactures",
        dataType: "json"
    }).done(function (msg) {
        var obj = $.parseJSON(JSON.stringify(msg));
        if (obj.DisplayType == "Dropdown") {
            $.each(obj.Manufacturers, function (i, p) {
                if (p.Id == manufacturerIds) {
                    $('#manufacturers').append($('<option selected></option>').val(p.Id).html(p.Name));
                }
                else {
                    $('#manufacturers').append($('<option></option>').val(p.Id).html(p.Name));
                }
            });
        }
        else {
            $.each(obj.Manufacturers, function (i, p) {
                if (p.Id == manufacturerIds) {
                    $('#manufacturers').append('<div class="checkbox"><input type="checkbox" data-type="manufacturer_chk" id="manufacturer_chk_' + p.Id + '" checked class="ManufactureCheckbox checkbox" name="manufacturer_chk" value="' + p.Id + '"><label for="manufacturer_chk_' + p.Id + '"><span>' + p.Name + '</span></label></div>');
                }
                else {
                    $('#manufacturers').append('<div class="checkbox"><input type="checkbox" class="ManufactureCheckbox checkbox" data-type="manufacturer_chk" id="manufacturer_chk_' + p.Id + '" name="manufacturer_chk" value="' + p.Id + '"><label for="manufacturer_chk_' + p.Id + '"><span>' + p.Name + '</span></label></div>');
                }
            });
            $(".ManufactureCheckbox").change(function () {
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
    if ($('.viewmode-icon.grid-view.active').length == 0) {
        $('.viewmode-icon.list-view.active').removeClass("active");
        $('.viewmode-icon.grid-view').addClass("active");
        viewMode = "grid";

    }
    else if ($('.viewmode-icon.list-view.active').length == 0) {
        //remove selected from grid
        $('.viewmode-icon.grid-view.active').removeClass("active");
        //add to list
        $('.viewmode-icon.list-view').addClass("active");
        viewMode = "list";
    }

    searchParameters();
}
$(document).ready(function () {
    if ($("#products-orderby").length > 0 && $("#products-pagesize").length > 0) {
        $("#products-orderby")[0].onchange = null;
        $("#products-pagesize")[0].onchange = null;
    }

    $(".filter-from-price, .filter-to-price").on("change", function () {
        //slider.slider("option", "values", [$(this).val().removeComma(), $(".filter-to-price").val().removeComma()]);
        priceSlider.noUiSlider.set([$(".filter-from-price").val().removeComma().toInt(), $(".filter-to-price").val().removeComma().toInt()]);
    });
});

function SearchSpecFilter() {
    if ($("#search_spec_input").val() != null && $("#search_spec_input").val() != undefined) {
        var textSearch = $("#search_spec_input").val().trim();
        if (textSearch !== '') {
            $(".check_specs_data > label > span:not(:contains('" + textSearch + "'))").parent().parent().hide();
        } else {
            $(".check_specs_data").show();
        }
    }

}

$(document).on('change', '#products-orderby,#products-pagesize', function (event) {
    event.preventDefault();
    searchParameters();
});
$(document).on('click', '.individual-page a,.next-page a,.previous-page a ,.last-page a, .first-page a', function (event) {
    event.preventDefault();

    searchParameters(getParameterByName("pagenumber", $(this)[0].href) == null ? 0 : getParameterByName("pagenumber", $(this)[0].href));
});

$(document).on('click', '.viewmode-icon', function (event) {
    event.preventDefault();
    console.log($(this)[0].title);
    selectViewMode($(this)[0].title);
});

function searchParameters(pageNumber) {
    $(".loading-ajax").show();
    spinner.spin(target[0]);
    var start = new Date().getTime();

    var specs = [];
    var attr = [];
    var categories = [];
    
    //check all options for each slider that is configured
    for (var i = 0; i < sliders.length; i++) {
        var values = sliders[i].slider.noUiSlider.get();

        if (sliders[i].id.indexOf('txt_') != -1) {
            if (parseInt(values[0]) == 0 & parseInt(values[1]) == text[sliders[i].id].length - 1) {

            }
            else {
                for (var ii = parseInt(values[0]); ii < parseInt(values[1]) + 1; ii++) {
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
                values[0] = values[0].slice(0, -sliders[i].postfix.length);
                values[1] = values[1].slice(0, -sliders[i].postfix.length);
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
    if ($('#specid').val() != undefined && $('#specid').val() !== '') {
        console.log(checkedSpecs);
        if (checkedSpecs.length == 0) {
            specs.push($('#specid').val().toInt());
            currentCheckbox = "chk_spec_" + $('#specid').val();
        }
    }
    $.merge(specs, checkedSpecs);


    var checkedCategories = $('input[data-type="chk_cate_"]:checked').map(function () {
        return parseInt(this.value);
    }).get();
    $.merge(categories, checkedCategories);

    var selectedAttr = $('select[data-type="cmb_attr_"]').map(function () {
        return parseInt($(this).find(":selected").val());
    }).get();
    $.merge(attr, selectedAttr);

    var selectedSpecs = $('select[data-type="cmb_spec_"]').map(function () {
        return parseInt($(this).find(":selected").val());
    }).get();
    $.merge(specs, selectedSpecs);
    if ($('.viewmode-icon.grid-view.active').length != 0) {
        viewMode = "grid";
    }
    if ($('.viewmode-icon.list-view.active').length != 0) {
        viewMode = "list";
    }

    if (pageNumber == undefined) {
        pageNumber = 0;
    }
    if (priceSlider != null) {
        if (priceSlider.noUiSlider == undefined) {
            var priceValues = ['0', '10000000'];
        }
        else {
            var priceValues = priceSlider.noUiSlider.get();
        }

    }


    var manufacturerIds = $('input[data-type="manufacturer_chk"]:checked').map(function () {
        return parseInt($(this).val());
    }).get().join(",");

    console.log(manufacturerIds);
    var sorting = getParameterByName('orderby', $("#products-orderby").val()) != null ? getParameterByName('orderby', $("#products-orderby").val()) : 0;
    var pagesize = getParameterByName('pagesize', $("#products-pagesize").val()) != null ? getParameterByName('pagesize', $("#products-pagesize").val()) : 0;
    if ('' != '') {
        var manufacturerIds = '';
        var dataPost = { specs: specs.join(","), attr: attr.join(","), viewmode: viewMode, pagesize: pagesize, pagenumber: pageNumber, orderby: sorting, manufacturerIds: manufacturerIds, minPrice: priceValues[0], maxPrice: priceValues[1] };
        $.ajax({
            method: "GET",
            url: "/Catalog/AjaxManufacturer",
            data: dataPost
        }).done(function (msg) {
            $(".loading-ajax").hide();

            if ($(".filter-ajax-result").length != 0) {
                $(".filter-ajax-result").eq(0).html(msg.ViewData);
            }
            else {
                $(".filter-ajax-result")[0].eq(0).html(msg.ViewData);
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
                var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '?specs=' + specs.join(",") + "&attr=" + attr.join(",") + "&categoryId=" + categoryId + "&viewmode=" + viewMode + "&pagesize=" + pagesize + "&pagenumber=" + pageNumber + "&manufacturerIds=" + manufacturerIds + "&orderby=" + sorting + "&minPrice=" + priceValues[0] + "&maxPrice=" + priceValues[1];

                //window.history.pushState({ path: newurl }, '', newurl);
            }

            //$('.page-body > .pager').remove();
            //spinner.spin(false);
        })
            .fail(function (error) {
                console.log('error');
                console.log(error);
            });
    }
    else if ('' != '') {
        var tagId = '';
        var dataPost = { specs: specs.join(","), attr: attr.join(","), productTagId: tagId, viewmode: viewMode, pagesize: pagesize, pagenumber: pageNumber, orderby: sorting, manufacturerIds: manufacturerIds, minPrice: priceValues[0], maxPrice: priceValues[1] };
        $.ajax({
            method: "GET",
            url: "/Catalog/AjaxTag",
            data: dataPost
        }).done(function (msg) {
            $(".loading-ajax").hide();

            if ($(".filter-ajax-result").length != 0) {
                $(".filter-ajax-result").eq(0).html(msg.ViewData);
            }
            else {
                $(".filter-ajax-result")[0].eq(0).html(msg.ViewData);
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
                var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '?specs=' + specs.join(",") + "&attr=" + attr.join(",") + "&categoryId=" + categoryId + "&viewmode=" + viewMode + "&pagesize=" + pagesize + "&pagenumber=" + pageNumber + "&manufacturerIds=" + manufacturerIds + "&orderby=" + sorting + "&minPrice=" + priceValues[0] + "&maxPrice=" + priceValues[1];
                //window.history.pushState({ path: newurl }, '', newurl);
            }

            //$('.page-body > .pager').remove();
            //spinner.spin(false);
        })
            .fail(function (error) {
                console.log('error');
                console.log(error);
            });

    }
    else if ('' != '') {
        var dataPost = { q: $("#q").val(), adv: $("#adv").val(), adv: $("#adv").val(), cid: $("#cid").val(), isc: $("#isc").val(), mid: $("#mid").val(), sid: $("#sid").val(), manufacturerIds: manufacturerIds, specs: specs.join(","), attr: attr.join(","), viewmode: viewMode, pagesize: pagesize, pagenumber: pageNumber, orderby: sorting, pf: $("#pf").val(), pt: $("#pt").val() };
        $.ajax({
            method: "GET",
            url: "/Catalog/AjaxSearch",
            data: dataPost
        }).done(function (msg) {
            $(".loading-ajax").hide();

            if ($(".filter-ajax-result").length != 0) {
                $(".filter-ajax-result").eq(0).html(msg.ViewData);
            }
            else {
                $(".filter-ajax-result")[0].eq(0).html(msg.ViewData);
            }
            //$('.page-body > .pager').remove();

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
        var searchKeyword = '';
        if ($("#q").val() != undefined && $("#q").val() != '') {
            searchKeyword = $("#q").val();
        }
        var dataPost = { q: searchKeyword, specs: specs.join(","), attr: attr.join(","), categoryId: categoryId, categoryIds: categories.join(","), viewmode: viewMode, pagesize: pagesize, pagenumber: pageNumber, orderby: sorting, manufacturerIds: manufacturerIds, minPrice: priceValues[0], maxPrice: priceValues[1] };
        $.ajax({
            method: "GET",
            url: "/Catalog/ProductFilterAjax",
            data: dataPost
        }).done(function (msg) {
            $(".loading-ajax").hide();
            if ($(".filter-ajax-result").length != 0) {
                $(".filter-ajax-result").eq(0).html(msg.ViewData);
            }
            else {
                $(".filter-ajax-result")[0].eq(0).html(msg.ViewData);
            }

            if ($('#groups_filter_specs').length != 0) {
                $('#groups_filter_specs').eq(0).html(msg.ViewDataFilterSpec);
            } else {
                $('#groups_filter_specs')[0].eq(0).html(msg.ViewDataFilterSpec);
            }
            if (msg.ViewDataFilterSpec != '') {
                $(".SpecCheckbox").change(function () {
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

            var checkedSpecSearch;
            var checkedAttrSearch;
            //loop all checkboxes and disable the checkboxes that cant be filtered
            //console.log(currentCheckbox);
            //var checkedSpecs = $('input[data-type="chk_spec_"]').map(function () {

            //    if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
            //        if ($(this).attr("name") != currentCheckbox) {
            //            $(this).attr("disabled", true);
            //            $(this).parent().addClass("disabled");
            //            $(this).parent().hide();

            //        }

            //    }
            //    else {
            //        $(this).parent().removeClass("disabled");
            //        $(this).removeAttr("disabled");
            //        $(this).parent().show();
            //    }
            //});
            //loop all checkboxes and disable the checkboxes that cant be filtered
            var checkedSpecs = $('input[data-type="chk_attr_"]').map(function () {
                if ($.inArray(parseInt(this.value), msg.FilterableProdAttr) == -1) {
                    if ($(this).attr("name") != currentCheckbox) {
                        $(this).parent().addClass("disabled");
                        $(this).parent().hide();
                    }
                }
                else {
                    $(this).parent().removeClass("disabled");
                    $(this).parent().show();

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
            //var inputSpecs = $(inputSpecSearch).map(function () {
            //    if (this.value != 0) {
            //        if ($.inArray(parseInt(this.value), msg.FilterableSpecAttr) == -1) {
            //            $(this).attr("disabled", true);
            //            $(this).parent().addClass("disabled");
            //            $(this).parent().hide();
            //        }
            //        else {
            //            $(this).parent().removeClass("disabled");
            //            $(this).removeAttr("disabled");
            //            $(this).parent().show();
            //        }
            //    }
            //});
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

                        $(this).parent().hide();
                    }
                    else {
                        $(this).parent().removeClass("disabled");
                        $(this).removeAttr("disabled");

                        $(this).parent().show();
                    }
                }
            });



            enableCheckbox = true;
            currentCheckbox = '';
            currentDropdown = '';
            currentManufacturer = '';
            resetDropdown = false;
            if (history.pushState) {
                var newurl = window.location.protocol +
                    "//" +
                    window.location.host +
                    window.location.pathname +
                    '?q=' +
                    searchKeyword;
                if (specs.length > 0) {
                    newurl = newurl + '&specs=' + specs.join(",");
                }
                if (attr.length > 0) {
                    newurl = newurl  + "&attr=" + attr.join(",");
                }
                //if (categoryId != null && categoryId !== '') {
                //    newurl = newurl   + "&categoryId=" + categoryId;
                //}
                if (categories.length > 0) {
                    newurl = newurl + "&categoryIds=" + categories.join(",");
                }
                if (viewMode != null && viewMode !== '' && viewMode !== 'grid') {
                    newurl = newurl   + "&viewmode=" + viewMode;
                }
                //if (pagesize != null && pagesize !== '') {
                //    newurl = newurl  + "&pagesize=" + pagesize;
                //}
                
                if (pageNumber != null && pageNumber !== '' && pageNumber !== 0) {
                    newurl = newurl  + "&pagenumber=" + pageNumber;
                }
                
                if (manufacturerIds != null && manufacturerIds !== '') {
                    newurl = newurl  + "&manufacturerIds=" + manufacturerIds;
                }
                
                if (sorting != null && sorting !== '' && sorting !== '0') {
                    newurl = newurl  + "&orderby=" + sorting;
                }
                if (priceValues[0] !== '0') {
                    newurl = newurl + "&minPrice=" + priceValues[0];
                }
                if (priceValues[1] !== '10000000') {
                    newurl = newurl + "&maxPrice=" + priceValues[1];
                }

                //window.history.pushState({ path: newurl }, '', newurl);
            }

            //$('.page-body > .pager').remove();
            spinner.spin(false);
        })
            .fail(function (error) {
                console.log('error');
                console.log(error);
            });
    }


}

