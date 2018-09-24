// Scripts for Infomedia HTML Renderer (c) Gorba AG

// Note: updateUrl is set by the HTML page before including this file
$(document).ready(requestUpdates);

var requestTimeout = 60 * 1000;  // 1 minute
var retryTimeout = 10 * 1000;  // 10 seconds
var altTime = 3 * 1000;  // 3 seconds
var blinkTime = 500;  // 0.5 seconds

var altTimerId = -1;
var blinkTimerId = -1;

function requestUpdates() {
    $.ajax({
        async: true,
        cache: false,
        url: updateUrl,
        dataType: 'json',
        success: handleUpdates,
        error: handleUpdateError,
        timeout: requestTimeout, // 1 minute
        type: 'GET'
    });
}

function handleUpdateError(xhr, status, error) {
    self.setTimeout(requestUpdates, retryTimeout);
}

function handleUpdates(data) {
    //try {
        log('Updates:' + JSON.stringify(data));
        if (data.Screen != null) {
            clearInterval(altTimerId);
            clearInterval(blinkTimerId);
            clearAllItems();
            $.each(data.Screen, function (index, element) {
                addElement(element, null);
            });
            altTimerId = setInterval(altTimer, altTime);
            blinkTimerId = setInterval(blinkTimer, blinkTime);
        }
        else if (data.Updates != null) {
            $.each(data.Updates, function (index, element) {
                var node = $('#' + element.Type + element.Id);
                switch (element.Type) {
                    case 'ImageItem':
                        updateImageItem(node, element);
                        break;
                    case 'TextItem':
                        updateTextItem(node, element);
                        break;
                    case 'IncludeItem':
                        updateIncludeItem(element);
                        break;
                }
            });
        }
    //} catch(e) {
        // ignore exception and continue silently
    //}
    
    requestUpdates();
}

function clearAllItems() {
    $('body').empty();
}

function clearItems(parentId) {
    var parentClass = '.parent_' + parentId;
    $(parentClass).remove();
}

function addElement(element, parentId) {
    var tag;
    switch (element.Type) {
        case 'ImageItem':
            tag = addImageItem(element);
            break;
        case 'TextItem':
            tag = addTextItem(element);
            break;
        case 'AnalogClockItem':
            tag = addAnalogClockItem(element);
            break;
        case 'IncludeItem':
            addIncludeItem(element);
            break;
    }

    if (tag != null) {
        var parentClass = "parent_" + parentId;
        tag.addClass(parentClass);
    }
}

function addImageItem(item) {
    log('addImageItem:' + JSON.stringify(item));
    return addImage(item.Type + item.Item.Id, item.Item, $('body'));
}

function addImage(id, item, parent) {
    var img = $('<img>');
    img.attr('id', id);
    img.css({
        position: 'absolute',
        left: item.X,
        top: item.Y,
        width: item.Width,
        height: item.Height,
        zIndex: item.ZIndex,
        visibility: item.Visible ? 'visible' : 'hidden'
    });
    if (item.Blink && item.Visible) {
        img.addClass('blinking');
    }
    // TODO: it would be nicer to delay the loading of the entire screen until all images are loaded,
    // like this we will just delay the showing of the images
    // Example: http://stackoverflow.com/questions/5623639/fire-an-event-after-preloading-images
    img.load(function () {
        parent.append(img);
    });
    img.attr('src', item.Filename);
    return img;
}

function updateImageItem(node, item) {
    switch (item.Property) {
        case 'Visible':
            node.css('visibility', item.Value ? 'visible' : 'hidden');
            break;
        case 'Filename':
            node.attr('src', item.Value);
            break;
    }
}

function addAnalogClockItem(item) {
    log('addAnalogClockItem:' + JSON.stringify(item));
    var wrapper = $('<div>');
    wrapper.attr('id', item.Type + item.Item.Id);
    wrapper.css({
        position: 'absolute',
        left: item.Item.X,
        top: item.Item.Y,
        width: item.Item.Width,
        height: item.Item.Height,
        zIndex: item.Item.ZIndex,
        visibility: item.Item.Visible ? 'visible' : 'hidden'
    });
    $('body').append(wrapper);

    var time = parseInt(item.Time);
    var hand;
    if (item.Item.Hour) {
        hand = addImage(item.Type + item.Item.Id + 'H', item.Item.Hour, wrapper);
        animateAnalogClockHand(item.Item.Hour, hand, time, 12, 3600);
    }
    if (item.Item.Minute) {
        hand = addImage(item.Type + item.Item.Id + 'M', item.Item.Minute, wrapper);
        animateAnalogClockHand(item.Item.Minute, hand, time, 60, 60);
    }
    if (item.Item.Seconds) {
        hand = addImage(item.Type + item.Item.Id + 'S', item.Item.Seconds, wrapper);
        animateAnalogClockHand(item.Item.Seconds, hand, time, 60, 1);
    }

    return wrapper;
}

function animateAnalogClockHand(hand, img, time, modulo, divider) {
    var seconds = time / 1000.0;
    var value = (seconds / divider) % modulo;
    var offset = value / modulo * 360;
    img.css({
        transformOrigin: (hand.CenterX * 100 / hand.Width) + '% ' + (hand.CenterY * 100 / hand.Height) + '%',
        textIndent: offset,
        transform: 'rotate(' + offset + 'deg)'
    });
    img.animate({
        textIndent: offset + 24 * 60 * 60 * 360 / modulo / divider
    }, {
        easing: 'linear',
        duration: 24 * 60 * 60 * 1000, // run for a maximum of 24 hours
        step: function (now, fx) {
            if (hand.Mode == 'Jump') {
                now = Math.floor(now * modulo / 360) * 360 / modulo;
            }
            else if (hand.Mode == 'CatchUp' && divider == 1) {
                now = Math.min((now % 360) / 58.5 * 60, 360);
            }
            $(this).css('transform', 'rotate(' + now + 'deg)');
        }
    });
}

function addIncludeItem(item) {
    log('addIncludeItem:' + JSON.stringify(item));
    $.each(item.Items, function (index, element) {
        addElement(element, item.Id);
    });
}

function updateIncludeItem(item) {
    if (item.Property == 'Items') {
        clearItems(item.Id);
        $.each(item.Value, function (index, element) {
            addElement(element, item.Id);
        });
    }
}

function addTextItem(item) {
    log('addTextItem:' + JSON.stringify(item));
    var root = $('<div>');
    root.css({
        position: 'absolute',
        left: item.Item.X,
        top: item.Item.Y,
        zIndex: item.Item.ZIndex,
        textAlign: item.Item.Align.toLowerCase(),
        visibility: item.Item.Visible ? 'visible' : 'hidden'
    });
    if (item.Item.Width > 0) {
        root.css('width', item.Item.Width);
    }
    if (item.Item.Height > 0) {
        root.css('height', item.Item.Width);
    }
    root.addClass('TextOverflow' + item.Item.Overflow);
    $('body').append(root);

    var altParent = root;
    if (item.Item.Overflow == 'Scale') {
        var scaleSpan = $('<span class="scale">');
        root.append(scaleSpan);
        altParent = scaleSpan;
    }
    else if (item.Item.Overflow == 'Scroll' || item.Item.Overflow == 'ScrollAlways') {
        var scrollSpan = $('<span class="scroll">');
        scrollSpan.css({
            position: 'relative'
        });
        root.append(scrollSpan);
        altParent = scrollSpan;
    }

    altParent.attr('id', item.Type + item.Item.Id);
    altParent.attr('item', JSON.stringify(item.Item));
    setAlternatives(altParent, item.Item, item.Alternatives);

    recalculateText(root);

    return root;
}

function updateTextItem(node, item) {
    switch (item.Property) {
        case 'Visible':
            node.css('visibility', item.Value ? 'visible' : 'hidden');
            break;
        case 'Text':
            node.empty();
            setAlternatives(node, JSON.parse(node.attr('item')), item.Value);
            var root = node.closest('div');
            recalculateText(root);
            break;
    }
}

function setAlternatives(parent, item, alternatives) {
    if (alternatives.length == 1) {
        addFormattedText(parent, item, alternatives[0]);
    } else {
        var first = true;
        var altsSpan = $('<span class="alts">');
        parent.append(altsSpan);
        $.each(alternatives, function (index, element) {
            var alt = $('<span>');
            alt.css('display', first ? 'inline' : 'none');
            altsSpan.append(alt);
            addFormattedText(alt, item, element);
            first = false;
        });
    }
}

function addFormattedText(parent, item, formattedText) {
    if ((item.Overflow == 'Clip' || item.Overflow == 'Overflow') && item.Align != 'Left') {
        var alignSpan = $('<span class="realign">');
        parent.append(alignSpan);
        parent = alignSpan;
    }

    $.each(formattedText.Parts, function (index, element) {
        if (element.Text == '\r\n') {
            var br = $('<br />');
            parent.append(br);
            return;
        }
        
        var span = $('<span>');
        span.css({
            color: element.Font.Color,
            fontFamily: element.Font.Face,
            fontSize: element.Font.Height,
            fontWeight: element.Font.Weight
        });
        if (element.Font.Italic) {
            span.css('font-style', 'italic');
        }
        if (element.Blink) {
            span.addClass('blinking');
        }
        span.append(element.Text);
        parent.append(span);
    });
}

function recalculateText(parent) {
    realignText(parent);
    scaleText(parent);
    scrollText(parent);
}

function realignText(parent) {
    parent.find('.realign').each(function (i) {
        var realign = $(this);
        var root = realign.closest('div');
        var diff = root.width() - realign.width();
        if (diff >= 0 || realign.css('left') != 'auto') {
            return;
        }

        $(this).css({
            position: 'relative',
            left: root.css('textAlign') == 'right' ? diff : (diff / 2)
        });
    });
}

function scaleText(parent) {
    parent.find('.scale').each(function (i) {
        var scale = $(this);
        var root = scale.closest('div');
        var targetWidth = root.width();
        var percentage = targetWidth * 1.0 / scale.width();

        // store the original fontSize
        scale.children('span').each(function (j) {
            $(this).attr('origFontSize', parseInt($(this).css('fontSize')));
        });

        while (scale.width() > targetWidth) {
            scale.children('span').each(function (j) {
                $(this).css('fontSize', Math.floor($(this).attr('origFontSize') * percentage));
            });
            percentage *= 0.98;
        }
    });
}

function scrollText(parent) {
    // todo: it might be better to use CSS3 instead of jQuery, we have to test performance
    parent.find('.scroll').each(function (i) {
        var scroll = $(this);
        var root = scroll.closest('div');
        var item = JSON.parse(scroll.attr('item'));

        var textWidth = scroll.width();
        var availWidth = root.width();

        $(this).find('.alts').children().each(function (j) {
            textWidth = Math.max(textWidth, $(this).width());
        });

        if (item.Overflow == 'ScrollAlways' || availWidth < textWidth) {
            var speed = item.ScrollSpeed;
            if (speed == 0) {
                return;
            }
            
            var origLeft = 0;
            var realign = root.find('.realign');
            if (realign.length) {
                origLeft -= parseInt(realign.css('left'));
            }
            if (speed < 0) {
                origLeft += availWidth;
            } else {
                origLeft -= textWidth;
            }
            var delta = availWidth + textWidth;
            var scroller = function () {
                scroll.css('left', origLeft);
                scroll.animate({
                    left: origLeft + delta * (speed / Math.abs(speed))
                }, {
                    easing: 'linear',
                    duration: delta * 1000 / Math.abs(speed),
                    complete: scroller // restart once we are done
                });
            };
            scroller();
        }
    });
}

function altTimer() {
    $('.alts').each(function (i) {
        $(this).children().each(function (j) {
            var alt = $(this);
            if (alt.css('display') == 'none') {
                return true;
            }

            alt.css('display', 'none');
            if (alt.next().length) {
                alt.next().css('display', 'inline');
            }
            else {
                alt.parent().children().first().css('display', 'inline');
            }

            recalculateText(alt);
            return false;
        });
    });
}

function blinkTimer() {
    $('.blinking').each(function (i) {
        $(this).css('visibility', $(this).css('visibility') == 'visible' ? 'hidden' : 'visible');
    });
}

function log(message) {
    if (console != null) {
        console.log(message);
    }
}
