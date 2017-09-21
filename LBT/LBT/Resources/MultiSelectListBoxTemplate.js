$(function () {{
    $('#{0}_leftValues option').sort(LBT.SortOptions).appendTo('#{0}_leftValues');
    $('#{0} option').sort(LBT.SortOptions).appendTo('#{0}');
    $('form').submit(function () {{
        $('#{0} option').attr('selected', 'selected');
    }});
}});

$('#{0}_btnLeft').click(function () {{
    var selectedItem = $('#{0} option:selected');
    $('#{0}_leftValues').append(selectedItem);
    $('#{0}_leftValues option').sort(LBT.SortOptions).appendTo('#{0}_leftValues');
}});

$('#{0}_btnRight').click(function () {{
    var selectedItem = $('#{0}_leftValues option:selected');
    $('#{0}').append(selectedItem);
    $('#{0} option').sort(LBT.SortOptions).appendTo('#{0}');
}});