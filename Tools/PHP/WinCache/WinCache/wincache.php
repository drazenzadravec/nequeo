<?php
/*
   +----------------------------------------------------------------------------------------------+
   | Windows Cache for PHP                                                                        |
   +----------------------------------------------------------------------------------------------+
   | Copyright (c) 2009, Microsoft Corporation. All rights reserved.                              |
   |                                                                                              |
   | Redistribution and use in source and binary forms, with or without modification, are         |
   | permitted provided that the following conditions are met:                                    |
   | - Redistributions of source code must retain the above copyright notice, this list of        |
   | conditions and the following disclaimer.                                                     |
   | - Redistributions in binary form must reproduce the above copyright notice, this list of     |
   | conditions and the following disclaimer in the documentation and/or other materials provided |
   | with the distribution.                                                                       |
   | - Neither the name of the Microsoft Corporation nor the names of its contributors may be     |
   | used to endorse or promote products derived from this software without specific prior written|
   | permission.                                                                                  |
   |                                                                                              |
   | THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS  |
   | OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF              |
   | MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE   |
   | COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,    |
   | EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE|
   | GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED   |
   | AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING    |
   | NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED |
   | OF THE POSSIBILITY OF SUCH DAMAGE.                                                           |
   +----------------------------------------------------------------------------------------------+
   | Module: wincache.php                                                                         |
   +----------------------------------------------------------------------------------------------+
   | Authors: Don Venkat Raman <don.raman@microsoft.com>                                          |
   |          Ruslan Yakushev <ruslany@microsoft.com>                                             |
   | Updated: Eric Stenson <ericsten@microsoft.com>                                               |
   +----------------------------------------------------------------------------------------------+
*/

/**
 * ======================== CONFIGURATION SETTINGS ==============================
 * If you do not want to use authentication for this page, set USE_AUTHENTICATION to 0.
 * If you use authentication then replace the default password.
 */
define('USE_AUTHENTICATION', 1);
define('USERNAME', 'wincache');
define('PASSWORD', 'wincache');

/**
 * The Basic PHP authentication will work only when IIS is configured to support 
 * Anonymous Authentication' and nothing else. If IIS is configured to support/use
 * any other kind of authentication like Basic/Negotiate/Digest etc, this will not work.
 * In that case use the array below to define the names of users in your 
 * domain/network/workgroup which you want to grant access to.
 */
$user_allowed = array('DOMAIN\user1', 'DOMAIN\user2', 'DOMAIN\user3');

/**
 * If the array contains string 'all', then all the users authenticated by IIS
 * will have access to the page. Uncomment the below line and comment above line
 * to grant access to all users who gets authenticated by IIS.
 */
/* $user_allowed = array('all'); */

/** ===================== END OF CONFIGURATION SETTINGS ========================== */

if ( !extension_loaded( 'wincache' ) )
{
    die('The extension WINCACHE (php_wincache.dll) is not loaded. No statistics to show.');
}

if ( USE_AUTHENTICATION == 1 ) {
    if (!empty($_SERVER['AUTH_TYPE']) && !empty($_SERVER['REMOTE_USER']) && strcasecmp($_SERVER['REMOTE_USER'], 'anonymous'))
    {
        if (!in_array(strtolower($_SERVER['REMOTE_USER']), array_map('strtolower', $user_allowed))
        && !in_array('all', array_map('strtolower', $user_allowed)))
        {
            echo 'You are not authorised to view this page. Please contact server admin to get permission. Exiting.';
            exit;
        }
    }
    else if ( !isset($_SERVER['PHP_AUTH_USER'] ) || !isset( $_SERVER['PHP_AUTH_PW'] ) ||
              $_SERVER['PHP_AUTH_USER'] != USERNAME || $_SERVER['PHP_AUTH_PW'] != PASSWORD ) {
        header( 'WWW-Authenticate: Basic realm="WINCACHE Log In!"' );
        header( 'HTTP/1.0 401 Unauthorized' );
        exit;
    }
    else if ( $_SERVER['PHP_AUTH_PW'] == 'wincache' )
    {
        echo "Please change the default password to get this page working. Exiting.";
        exit;
    }
}

define('IMG_WIDTH', 320);
define('IMG_HEIGHT', 220);
define('SUMMARY_DATA', 1);
define('FCACHE_DATA', 3); // File cache
define('UCACHE_DATA', 4); // User cache
define('SCACHE_DATA', 5); // Session cache
define('RCACHE_DATA', 6); // Resolve file cache
define('BAR_CHART', 1);
define('PIE_CHART', 2);
define('PATH_MAX_LENGTH', 45);
define('INI_MAX_LENGTH', 45);
define('SUBKEY_MAX_LENGTH', 90);
define('CACHE_MAX_ENTRY', 250);

// WinCache settings that are used for debugging purposes
$settings_to_hide = array( 'wincache.localheap', 'wincache.debuglevel', 'wincache.olocaltest' );

// Input parameters check
$PHP_SELF = isset( $_SERVER['PHP_SELF'] ) ? htmlentities( strip_tags( $_SERVER['PHP_SELF'],'' ), ENT_QUOTES, 'UTF-8' ) : '';
$page = isset( $_GET['page'] ) ? $_GET['page'] : SUMMARY_DATA;
if ( !is_numeric( $page ) || $page < SUMMARY_DATA || $page > RCACHE_DATA )
    $page = SUMMARY_DATA;

$img = 0;
if ( isset( $_GET['img'] ) && is_numeric( $_GET['img'] ) ) {
    $img = $_GET['img'];
    if ( $img < FCACHE_DATA || $img > RCACHE_DATA)
        $img = 0;
}
$chart_type = BAR_CHART;
if ( isset( $_GET['type'] ) && is_numeric( $_GET['type'] ) ) {
    $chart_type = $_GET['type'];
    if ( $chart_type < BAR_CHART || $chart_type > PIE_CHART)
        $chart_type = BAR_CHART;
}
$chart_param1 = 0;
if ( isset( $_GET['p1'] ) && is_numeric( $_GET['p1'] ) ) {
    $chart_param1 = $_GET['p1'];
    if ( $chart_param1 < 0 ) 
        $chart_param1 = 0;
    else if ( $chart_param1 > PHP_INT_MAX )
        $chart_param1 = PHP_INT_MAX;
}
$chart_param2 = 0;
if ( isset( $_GET['p2'] ) && is_numeric( $_GET['p2'] ) ) {
    $chart_param2 = $_GET['p2'];
    if ( $chart_param2 < 0 ) 
        $chart_param2 = 0;
    else if ( $chart_param2 > PHP_INT_MAX )
        $chart_param2 = PHP_INT_MAX;
}

$show_all_ucache_entries = 0;
if ( isset( $_GET['all'] ) && is_numeric( $_GET['all'] ) ) {
    $show_all_ucache_entries = $_GET['all'];
    if ( $show_all_ucache_entries < 0 || $show_all_ucache_entries > 1)
        $show_all_ucache_entries = 0;
}

$clear_user_cache = 0;
if ( isset( $_GET['clc'] ) && is_numeric( $_GET['clc'] ) ) {
    $clear_user_cache = $_GET['clc'];
    if ( $clear_user_cache < 0 || $clear_user_cache > 1)
        $clear_user_cache = 0;
}

$ucache_key = null;
if ( isset( $_GET['key'] ) )
    $ucache_key = $_GET['key'];
// End of input parameters check

// Initialize global variables
$user_cache_available = function_exists('wincache_ucache_info') && !strcmp( ini_get( 'wincache.ucenabled' ), "1" );
$session_cache_available = function_exists('wincache_scache_info') && !strcasecmp( ini_get( 'session.save_handler' ), "wincache" );
$fcache_mem_info = null;
$fcache_file_info = null;
$fcache_summary_info = null;
$rpcache_mem_info = null;
$rpcache_file_info = null;
$ucache_mem_info = null;
$ucache_info = null;
$scache_mem_info = null;
$scache_info = null;
$sort_key = null;

if ( $session_cache_available && ( $page == SUMMARY_DATA || $page == SCACHE_DATA ) ){
    @session_name('WINCACHE_SESSION');
    session_start();
}

function cmp($a, $b)
{
    global $sort_key;
    if ( $sort_key == 'file_name' )
        return strcmp( get_trimmed_filename( $a[$sort_key], PATH_MAX_LENGTH ), get_trimmed_filename( $b[$sort_key], PATH_MAX_LENGTH ) );
    else if ( $sort_key == 'resolve_path' )
        return strcmp( get_trimmed_string( $a[$sort_key], PATH_MAX_LENGTH ), get_trimmed_string( $b[$sort_key], PATH_MAX_LENGTH ) );
    else
        return 0;
}

function convert_bytes_to_string( $bytes ) {
    $units = array( 0 => 'B', 1 => 'kB', 2 => 'MB', 3 => 'GB' );
    $log = log( $bytes, 1024 );
    $power = (int) $log;
    $size = pow(1024, $log - $power);
    return round($size, 2) . ' ' . $units[$power];
}

function seconds_to_words( $seconds ) {
    /*** return value ***/
    $ret = "";

    /*** get the hours ***/
    $hours = intval(intval( $seconds ) / 3600);
    if ( $hours > 0 ) {
        $ret .= "$hours hours ";
    }
    /*** get the minutes ***/
    $minutes = bcmod( ( intval( $seconds ) / 60 ), 60 );
    if( $hours > 0 || $minutes > 0 ) {
        $ret .= "$minutes minutes ";
    }

    /*** get the seconds ***/
    $seconds = bcmod( intval( $seconds ), 60 );
    $ret .= "$seconds seconds";

    return $ret;
}

function get_trimmed_filename( $filepath, $max_len ) {
    if ($max_len <= 0) die ('The maximum allowed length must be bigger than 0');
    
    $result = basename( $filepath );
    if ( strlen( $result ) > $max_len ) 
        $result = substr( $result, -1 * $max_len );
        
    return $result;
}

function get_trimmed_string( $input, $max_len ) {
    if ($max_len <= 3) die ('The maximum allowed length must be bigger than 3');
    
    $result = $input;
    if ( strlen( $result ) > $max_len ) 
        $result = substr( $result, 0, $max_len - 3 ). '...';
        
    return $result;
}

function get_trimmed_ini_value( $input, $max_len, $separators = array('|', ',') ) {
    if ($max_len <= 3) die ('The maximum allowed length must be bigger than 3');
    
    $result = $input;
    $lastindex = 0;
    if ( strlen( $result ) > $max_len ) {
        $result = substr( $result, 0, $max_len - 3 ).'...';
        if ( !is_array( $separators ) ) die( 'The separators must be in an array' );
        foreach ( $separators as $separator ) {
            $index = strripos( $result, $separator );
            if ( $index !== false  && $index > $lastindex )
                $lastindex = $index;
        }
        if ( 0 < $lastindex && $lastindex < ( $max_len - 3 ) )
            $result = substr( $result, 0, $lastindex + 1 ).'...';
    }
    return $result;
}

function get_fcache_summary( $entries ) {
    $result = array();
    $result['total_size'] = 0;
    $result['oldest_entry'] = '';
    $result['recent_entry'] = '';

    if ( isset( $entries ) && count( $entries ) > 0 && isset( $entries[1]['file_name'] ) ) {
        foreach ( (array)$entries as $entry ) {
            $result['total_size'] += $entry['file_size'];
        }
    }
    return $result;
}

function get_chart_title( $chart_data )
{
    $chart_title = '';
    switch( $chart_data ) {
        case RCACHE_DATA: {
            $chart_title = 'Resolve Cache';
            break;
        }
        case FCACHE_DATA: {
            $chart_title = 'File Cache';
            break;
        }
        case UCACHE_DATA: {
            $chart_title = 'User Cache';
            break;
        }
        case SCACHE_DATA: {
            $chart_title = 'Session Cache';
        }
    }
    return $chart_title;
}

function gd_loaded() {
    return extension_loaded( 'gd' );
}

if ( $img > 0 ) {
    if ( !gd_loaded() )
        exit( 0 );

    function create_hit_miss_chart( $width, $height, $hits, $misses, $title = 'Hits & Misses (in %)' ) {
        
        $hit_percent = 0;
        $miss_percent = 0;
        if ( $hits < 0 ) $hits = 0;
        if ( $misses < 0 ) $misses = 0;
        if ( $hits > 0 || $misses > 0 ) {
            $hit_percent = round( $hits / ( $hits + $misses ) * 100, 2 );
            $miss_percent = round( $misses / ( $hits + $misses ) * 100, 2 );
        }
        $data = array( 'Hits' => $hit_percent, 'Misses' => $miss_percent );
        
        $image = imagecreate( $width, $height ); 

        // colors 
        $white = imagecolorallocate( $image, 0xFF, 0xFF, 0xFF );
        $phpblue = imagecolorallocate( $image, 0x5C, 0x87, 0xB2 ); 
        $black = imagecolorallocate( $image, 0x00, 0x00, 0x00 ); 
        $gray = imagecolorallocate( $image, 0xC0, 0xC0, 0xC0 );

        $maxval = max( $data );
        $nval = sizeof( $data );

        // draw something here
        $hmargin = 38; // left horizontal margin for y-labels 
        $vmargin = 20; // top (bottom) vertical margin for title (x-labels) 

        $base = floor( ( $width - $hmargin ) / $nval );

        $xsize = $nval * $base - 1; // x-size of plot
        $ysize = $height - 2 * $vmargin; // y-size of plot
    
        // plot frame 
        imagerectangle( $image, $hmargin, $vmargin, $hmargin + $xsize, $vmargin + $ysize, $black ); 

        // top label
        $titlefont = 3;
        $txtsize = imagefontwidth( $titlefont ) * strlen( $title );
        $xpos = (int)( $hmargin + ( $xsize - $txtsize ) / 2 );
        $xpos = max( 1, $xpos ); // force positive coordinates 
        $ypos = 3; // distance from top 
        imagestring( $image, $titlefont, $xpos, $ypos, $title , $black ); 

        // grid lines
        $labelfont = 2;
        $ngrid = 4;

        $dydat = 100 / $ngrid;
        $dypix = $ysize / $ngrid;

        for ( $i = 0; $i <= $ngrid; $i++ ) {
            $ydat = (int)( $i * $dydat );
            $ypos = $vmargin + $ysize - (int)( $i * $dypix );
        
            $txtsize = imagefontwidth( $labelfont ) * strlen( $ydat );
            $txtheight = imagefontheight( $labelfont );
        
            $xpos = (int)( ( $hmargin - $txtsize) / 2 );
            $xpos = max( 1, $xpos );
        
            imagestring( $image, $labelfont, $xpos, $ypos - (int)( $txtheight/2 ), $ydat, $black );
        
            if ( !( $i == 0 ) && !( $i >= $ngrid ) ) 
                imageline( $image, $hmargin - 3, $ypos, $hmargin + $xsize, $ypos, $gray ); 
                // don't draw at Y=0 and top 
        }

        // graph bars
        // columns and x labels 
        $padding = 30; // half of spacing between columns 
        $yscale = $ysize / ( $ngrid * $dydat ); // pixels per data unit 

        for ( $i = 0; list( $xval, $yval ) = each( $data ); $i++ ) { 

            // vertical columns 
            $ymax = $vmargin + $ysize; 
            $ymin = $ymax - (int)( $yval * $yscale ); 
            $xmax = $hmargin + ( $i + 1 ) * $base - $padding; 
            $xmin = $hmargin + $i * $base + $padding; 

            imagefilledrectangle( $image, $xmin, $ymin, $xmax, $ymax, $phpblue ); 

            // x labels 
            $xlabel = $xval.': '.$yval.'%';
            $txtsize = imagefontwidth( $labelfont ) * strlen( $xlabel );

            $xpos = ( $xmin + $xmax - $txtsize ) / 2;
            $xpos = max( $xmin, $xpos ); 
            $ypos = $ymax + 3; // distance from x axis

            imagestring( $image, $labelfont, $xpos, $ypos, $xlabel, $black ); 
        }
        return $image;
    }
    
    function create_used_free_chart( $width, $height, $used_memory, $free_memory, $title = 'Free & Used Memory (in %)' ) {
        // Check the input parameters to avoid division by zero and weird cases
        if ( $free_memory <= 0 && $used_memory <= 0 ) {
            $free_memory = 1;
            $used_memory = 0;
        }
        
        $centerX = 120;
        $centerY = 120;
        $diameter = 120;

        $hmargin = 5; // left (right) horizontal margin
        $vmargin = 20; // top (bottom) vertical margin

        $image = imagecreate( $width, $height );

        // colors 
        $white = imagecolorallocate( $image, 0xFF, 0xFF, 0xFF );
        $black = imagecolorallocate( $image, 0x00, 0x00, 0x00 );
        $pie_color[1] = imagecolorallocate($image, 0x5C, 0x87, 0xB2);
        $pie_color[2] = imagecolorallocate($image, 0xCB, 0xE1, 0xEF);
        $pie_color[3] = imagecolorallocate($image, 0xC0, 0xC0, 0xC0);

        // Label font size
        $labelfont = 2;
        $hfw = imagefontwidth( $labelfont );
        $vfw = imagefontheight( $labelfont );

        // Border
        imagerectangle( $image, $hmargin, $vmargin, $width - $hmargin, $height - $vmargin, $black ); 

        // Top label
        $titlefont = 3;
        $txtsize = imagefontwidth( $titlefont ) * strlen( $title );
        $hpos = (int)( ($width - $txtsize) / 2 );
        $vpos = 3; // distance from top 
        imagestring( $image, $titlefont, $hpos, $vpos, $title , $black );

        $total = 0;
        $n = 0;
        $items = array('Used memory' => $used_memory, 'Free memory' => $free_memory);

        //read the arguments into different arrays:
        foreach( $items as $key => $val ) {
            $n++;
            $label[$n] = $key;
            $value[$n] = $val;
            $total += $val;
            $arc_dec[$n] = $total*360;
            $arc_rad[$n] = $total*2*pi();
        }

        //the base:
        $arc_rad[0] = 0;
        $arc_dec[0] = 0;

        //count the labels:
        for ( $i = 1; $i <= $n; $i++ ) {
            
            //calculate the percents:
            $perc[$i] = $value[$i] / $total;
            $percstr[$i] = (string) number_format( $perc[$i] * 100, 2 )."%";
            //label with percentage:
            $label[$i] = $percstr[$i];

            //calculate the arc and line positions:
            $arc_rad[$i] = $arc_rad[$i] / $total;
            $arc_dec[$i] = $arc_dec[$i] / $total;
            $hpos = round( $centerX + ( $diameter / 2 ) * sin( $arc_rad[$i] ) );
            $vpos = round( $centerY + ( $diameter / 2 ) * cos( $arc_rad[$i] ) );
            imageline( $image, $centerX, $centerY, $hpos, $vpos, $black );
            imagearc( $image, $centerX, $centerY, $diameter, $diameter, $arc_dec[$i-1], $arc_dec[$i], $black );

            //calculate the positions for the labels:
            $arc_rad_label = $arc_rad[$i-1] + 0.5 * $perc[$i] * 2 * pi();
            $hpos = $centerX + 1.1 * ( $diameter / 2 ) * sin( $arc_rad_label );
            $vpos = $centerY + 1.1 * ( $diameter / 2 ) * cos( $arc_rad_label );
            if ( ( $arc_rad_label > 0.5 * pi() ) && ( $arc_rad_label < 1.5 * pi() ) ) {
                $vpos = $vpos - $vfw;
            }
            if ( $arc_rad_label > pi() ) {
                $hpos = $hpos - $hfw * strlen( $label[$i] );
            }
            //display the labels:
            imagestring($image, $labelfont, $hpos, $vpos, $label[$i], $black);
        }

        //fill the parts with their colors:
        for ( $i = 1; $i <= $n; $i++ ) {
            if ( round($arc_dec[$i] - $arc_dec[$i-1]) != 0 ) {
                $arc_rad_label = $arc_rad[$i - 1] + 0.5 * $perc[$i] * 2 * pi();
                $hpos = $centerX + 0.8 * ( $diameter / 2 ) * sin( $arc_rad_label );
                $vpos = $centerY + 0.8 * ( $diameter / 2 ) * cos( $arc_rad_label );
                imagefilltoborder( $image, $hpos, $vpos, $black, $pie_color[$i] );
            }
        }

        // legend
        $hpos = $centerX + 1.1 * ($diameter / 2) + $hfw * strlen( '50.00%' );
        $vpos = $centerY - ($diameter / 2);
        $i = 1;
        $thumb_size = 5;
        foreach ($items as $key => $value){
            imagefilledrectangle( $image, $hpos, $vpos, $hpos + $thumb_size, $vpos + $thumb_size, $pie_color[$i++] );
            imagestring( $image, $labelfont, $hpos + $thumb_size + 5, $vpos, $key, $black );
            $vpos += $vfw + 2;
        }
        return $image;
    }

    $png_image = null;
    $chart_title = get_chart_title($img);
    
    if ( $chart_type == PIE_CHART ){
        $png_image = create_used_free_chart( IMG_WIDTH, IMG_HEIGHT, $chart_param1, $chart_param2, 'Memory Usage by '.$chart_title.' (in %)' );
    }
    else{
        $png_image = create_hit_miss_chart( IMG_WIDTH, IMG_HEIGHT,  $chart_param1, $chart_param2, $chart_title.' Hits & Misses (in %)' );
    }

    if ( $png_image !== null ) {
        // flush image 
        header('Content-type: image/png');
        imagepng($png_image);
        imagedestroy($png_image); 
    }
    exit;
}

function get_chart_markup( $data_type, $chart_type, $chart_param1, $chart_param2 ) {
    global    $PHP_SELF;

    $result = '';
    $alt_title = '';

    if ( gd_loaded() ){
        $alt_title = get_chart_title( $data_type );
        if ( $alt_title == '' )
            return '';

        if ( $chart_type == BAR_CHART )
            $alt_title .= ' hit and miss percentage chart';
        elseif ( $chart_type == PIE_CHART )
            $alt_title .= ' memory usage percentage chart';
        else
            return '';

        $result = '<img src="'.$PHP_SELF;
        $result .= '?img='.$data_type.'&amp;type='.$chart_type;
        $result .= '&amp;p1='.$chart_param1.'&amp;p2='.$chart_param2.'" ';
        $result .= 'alt="'.$alt_title.'" width="'.IMG_WIDTH.'" height="'.IMG_HEIGHT.'" />';
    }
    else {
        $result = '<p class="notice">Enable GD library (<em>php_gd2.dll</em>) in order to see the charts.</p>';
    }

    return $result;
}

function cache_scope_text( $is_local )
{
    return ( $is_local == true ) ? 'local' : 'global';
}

function init_cache_info( $cache_data = SUMMARY_DATA )
{
    global  $fcache_mem_info,
            $fcache_file_info,
            $fcache_summary_info,
            $rpcache_mem_info,
            $rpcache_file_info,
            $ucache_mem_info,
            $ucache_info,
            $scache_mem_info,
            $scache_info,
            $user_cache_available,
            $session_cache_available;

    if ( $cache_data == SUMMARY_DATA || $cache_data == FCACHE_DATA ) {
        $fcache_mem_info = wincache_fcache_meminfo();
        $fcache_file_info = wincache_fcache_fileinfo();
        $fcache_summary_info = get_fcache_summary( $fcache_file_info['file_entries'] );
    }
    if ( $cache_data == SUMMARY_DATA || $cache_data == RCACHE_DATA ){
        $rpcache_mem_info = wincache_rplist_meminfo();
        $rpcache_file_info = wincache_rplist_fileinfo();
    }
    if ( $user_cache_available && ( $cache_data == SUMMARY_DATA || $cache_data == UCACHE_DATA ) ){
        $ucache_mem_info = wincache_ucache_meminfo();
        $ucache_info = wincache_ucache_info();
    }
    if ( $session_cache_available && ( $cache_data == SUMMARY_DATA || $cache_data == SCACHE_DATA ) ){
        $scache_mem_info = wincache_scache_meminfo();
        $scache_info = wincache_scache_info();
    }
}

if ( USE_AUTHENTICATION && $user_cache_available && $clear_user_cache ){
    wincache_ucache_clear();
    header('Location: '.$PHP_SELF.'?page='.UCACHE_DATA);
    exit;
}

?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head>
<style type="text/css">
body {
    background-color: #ffffff;
    color: #000000;
    font-family: sans-serif;
    font-size: 0.8em;
}
h1 {
    font-size: 2em;
}
#content {
    width: 960px;
    margin: 5px;
}
#header {
    color: #ffffff;
    border: 1px solid black;
    background-color: #5C87B2;
    margin-bottom: 1em;
    padding: 1em 2em;
}
/*The #menu element credits: */
/*Credits: Dynamic Drive CSS Library */
/*URL: http://www.dynamicdrive.com/style/ */
#menu {
    width: 100%;
    overflow: hidden;
    border-bottom: 1px solid black;
    margin-bottom: 1em; /*bottom horizontal line that runs beneath tabs*/
}
#menu ul {
    margin: 0;
    padding: 0;
    padding-left: 10px; /*offset of tabs relative to browser left edge*/;
    font-weight: bold;
    font-size: 1.2em;
    list-style-type: none;
}
#menu li {
    display: inline;
    margin: 0;
}
#menu li a {
    float: left;
    display: block;
    text-decoration: none;
    margin: 0;
    padding: 7px 8px;
    border-right: 1px solid white; /*padding inside each tab*/
    color: white; /*right divider between tabs*/
    background: #5C87B2; /*background of tabs (default state)*/
}
#menu li a:visited {
    color: white;
}
#menu li a:hover, #menu li.selected a {
    background: #336699;
}
/*The end of the menu elements credits */
#panel{
    float: left;
    width: 100%;
    margin-bottom: 2em;
    border: 1px solid black;
}
#panel_header{
    background-color: #5C87B2;
    font-weight: bold;
    color: #ffffff;
    border-bottom: 1px solid black;
    padding: 0.5em;
}
#panel_body{
    background-color: #E7E7E7;
    padding: 0.5em;
    white-space: 
}
pre {
    white-space: pre-wrap; /* css-3 */
    white-space: -moz-pre-wrap !important; /* Mozilla, since 1999 */
    white-space: -pre-wrap; /* Opera 4-6 */
    white-space: -o-pre-wrap; /* Opera 7 */
    word-wrap: break-word; /* Internet Explorer 5.5+ */
}
.overview{
    float: left;
    width: inherit;
    margin-bottom: 2em;
}
.list{
    float: left;
    width: 100%;
    margin-bottom: 2em;
}
.wideleftpanel{
    float: left;
    width: 520px;
    margin-right: 20px;
}
.widerightpanel{
    float: left;
    width: 420px;
}
.leftpanel{
    float: left;
    width: 310px;
}
.rightpanel{
    float:left;
    width: 320px;
    margin-left: 5px;
}
.extra_margin{
    margin-top: 20px;
}
table {
    border-collapse: collapse;
}
td, th {
    border: 1px solid black;
    vertical-align: baseline;
}
th {
    background-color: #5C87B2;
    font-weight: bold;
    color: #ffffff;
}
.e {
    background-color: #cbe1ef;
    font-weight: bold;
    color: #000000;
    width: 40%;
}
.leftpanel .e{
    width: 50%;
}
.v {
    background-color: #E7E7E7;
    color: #000000;
}
.n{
    background-color: #FFFF00;
    color: #000000;
    font-weight: bold;
}
.notice {
    display: block;
    margin-top: 1.5em;
    padding: 1em;
    background-color: #ffffe0;
    border: 1px solid #dddddd;
}
.clear{
    clear: both;
}
</style>
<title>Windows Cache Extension for PHP - Statistics</title>
</head>

<body>

<div id="content">
    <div id="header">
        <h1>Windows Cache Extension for PHP - Statistics</h1>
    </div>
    <div id="menu">
        <ul>
            <li <?php echo ($page == SUMMARY_DATA)? 'class="selected"' : ''; ?>><a href="<?php echo $PHP_SELF, '?page=', SUMMARY_DATA; ?>">Summary</a></li>
            <li <?php echo ($page == FCACHE_DATA)? 'class="selected"' : ''; ?>><a href="<?php echo $PHP_SELF, '?page=', FCACHE_DATA; ?>">File System Cache</a></li>
            <li <?php echo ($page == UCACHE_DATA)? 'class="selected"' : ''; ?>><a href="<?php echo $PHP_SELF, '?page=', UCACHE_DATA; ?>">User Cache</a></li>
            <li <?php echo ($page == SCACHE_DATA)? 'class="selected"' : ''; ?>><a href="<?php echo $PHP_SELF, '?page=', SCACHE_DATA; ?>">Session Cache</a></li>
            <li <?php echo ($page == RCACHE_DATA)? 'class="selected"' : ''; ?>><a href="<?php echo $PHP_SELF, '?page=', RCACHE_DATA; ?>">Resolve Path Cache</a></li>
        </ul>
    </div>
<?php if ( $page == SUMMARY_DATA ) { 
    init_cache_info( SUMMARY_DATA );
?>
    <div class="overview">
        <div class="wideleftpanel">
            <table style="width: 100%">
                <tr>
                    <th colspan="2">General Information</th>
                </tr>
                <tr>
                    <td class="e">WinCache version</td>
                    <td class="v"><?php echo phpversion('wincache'); ?></td>
                </tr>
                <tr>
                    <td class="e">PHP version</td>
                    <td class="v"><?php echo phpversion(); ?></td>
                </tr>
                <tr title="<?php echo $_SERVER['DOCUMENT_ROOT']; ?>">
                    <td class="e">Document root</td>
                    <td class="v"><?php echo get_trimmed_string( $_SERVER['DOCUMENT_ROOT'], PATH_MAX_LENGTH ); ?></td>
                </tr>
                <tr title="<?php echo isset( $_SERVER['PHPRC'] ) ? $_SERVER['PHPRC'] : 'Not defined'; ?>">
                    <td class="e">PHPRC</td>
                    <td class="v"><?php echo isset( $_SERVER['PHPRC'] ) ? get_trimmed_string( $_SERVER['PHPRC'], PATH_MAX_LENGTH ) : 'Not defined'; ?></td>
                </tr>
                <tr>
                    <td class="e">Server software</td>
                    <td class="v"><?php echo isset( $_SERVER['SERVER_SOFTWARE'] ) ? $_SERVER['SERVER_SOFTWARE']: 'Not set'; ?></td>
                </tr>
                <tr>
                    <td class="e">Operating System</td>
                    <td class="v"><?php echo php_uname( 's' ), ' ', php_uname( 'r' ); ?></td>
                </tr>
                  <tr>
                    <td class="e">Processor information</td>
                    <td class="v"><?php echo isset( $_SERVER['PROCESSOR_IDENTIFIER'] ) ? $_SERVER['PROCESSOR_IDENTIFIER']: 'Not set'; ?></td>
                </tr>
                <tr>
                    <td class="e">Number of processors</td>
                    <td class="v"><?php echo isset( $_SERVER['NUMBER_OF_PROCESSORS'] ) ? $_SERVER['NUMBER_OF_PROCESSORS']: 'Not set'; ?></td>
                </tr>
                <tr>
                    <td class="e">Machine name</td>
                    <td class="v"><?php echo (getenv( 'COMPUTERNAME' ) != FALSE) ? getenv( 'COMPUTERNAME' ) : 'Not set'; ?></td>
                </tr>
                <tr>
                    <td class="e">Host name</td>
                    <td class="v"><?php echo isset( $_SERVER['HTTP_HOST'] ) ? $_SERVER['HTTP_HOST'] : 'Not set'; ?></td>
                </tr>
                <tr>
                    <td class="e">PHP session handler</td>
                    <td class="v"><?php echo ini_get( 'session.save_handler' ); ?></td>
                </tr>
                <tr>
                    <td class="e">Application Pool ID</td>
                    <td class="v"><?php echo (getenv( 'APP_POOL_ID' ) != FALSE) ? getenv( 'APP_POOL_ID') : 'Not available'; ?></td>
                </tr>
                <tr>
                    <td class="e">Site ID</td>
                    <td class="v"><?php echo isset( $_SERVER['INSTANCE_ID'] ) ? $_SERVER['INSTANCE_ID'] : 'Not available'; ?></td>
                </tr>
                <tr>
                    <td class="e">FastCGI impersonation</td>
                    <td class="v"><?php echo (ini_get( 'fastcgi.impersonate' ) === '1') ? 'enabled' : 'disabled'; ?></td>
                </tr>
            </table>
        </div>
        <div class="widerightpanel">
            <table style="width:100%">
                <tr>
                    <th colspan="2">Cache Settings</th>
                </tr>
<?php 
foreach ( ini_get_all( 'wincache' ) as $ini_name => $ini_value) {
    // Do not show the settings used for debugging
    if ( in_array( $ini_name, $settings_to_hide ) )
        continue;
    echo '<tr title="', $ini_value['local_value'], '"><td class="e">', $ini_name, '</td><td class="v">';
    if ( !is_numeric( $ini_value['local_value'] ) )
        echo get_trimmed_ini_value( $ini_value['local_value'], INI_MAX_LENGTH );
    else
        echo $ini_value['local_value'];
    echo '</td></tr>', "\n";
} 
?>
            </table>
        </div>
    </div>
    <div class="overview">
        <div class="leftpanel extra_margin">
            <table style="width: 100%">
                <tr>
                    <th colspan="2">File Cache Overview</th>
                </tr>
                <tr>
                    <td class="e">Cache uptime</td>
                    <td class="v"><?php echo ( isset( $fcache_file_info['total_cache_uptime'] ) ) ? seconds_to_words( $fcache_file_info['total_cache_uptime'] ) : 'Unknown'; ?></td>
                </tr>                
                <tr>
                    <td class="e">Cached files</td>
                    <td class="v"><a href="<?php echo $PHP_SELF, '?page=', FCACHE_DATA, '#filelist'; ?>"><?php echo $fcache_file_info['total_file_count']; ?></a></td>
                </tr>
                <tr>
                    <td class="e">Total files size</td>
                    <td class="v"><?php echo convert_bytes_to_string( $fcache_summary_info['total_size'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Hits</td>
                    <td class="v"><?php echo $fcache_file_info['total_hit_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Misses</td>
                    <td class="v"><?php echo $fcache_file_info['total_miss_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Total memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $fcache_mem_info['memory_total'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Available memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $fcache_mem_info['memory_free'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Memory overhead</td>
                    <td class="v"><?php echo convert_bytes_to_string( $fcache_mem_info['memory_overhead'] ); ?></td>
                </tr>
            </table>
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( FCACHE_DATA, BAR_CHART, $fcache_file_info['total_hit_count'], $fcache_file_info['total_miss_count'] ); ?>
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( FCACHE_DATA, PIE_CHART, $fcache_mem_info['memory_total'] - $fcache_mem_info['memory_free'], $fcache_mem_info['memory_free'] ); ?>
        </div>
    </div>
    <div class="overview">
        <?php if ( $user_cache_available ) {?>
        <div class="leftpanel extra_margin">
            <table style="width: 100%">
                <tr>
                    <th colspan="2">User Cache Overview</th>
                </tr>
                <tr>
                    <td class="e">Cache scope</td>
                    <td class="v"><?php echo ( isset( $ucache_info['is_local_cache'] ) ) ? cache_scope_text( $ucache_info['is_local_cache'] ) : 'Unknown'; ?></td>
                </tr>
                <tr>
                    <td class="e">Cache uptime</td>
                    <td class="v"><?php echo ( isset( $ucache_info['total_cache_uptime'] ) ) ? seconds_to_words( $ucache_info['total_cache_uptime'] ) : 'Unknown'; ?></td>
                </tr>                
                <tr>
                    <td class="e">Cached entries</td>
                    <td class="v"><a href="<?php echo $PHP_SELF, '?page=', UCACHE_DATA, '#filelist'; ?>"><?php echo $ucache_info['total_item_count']; ?></a></td>
                </tr>
                <tr>
                    <td class="e">Hits</td>
                    <td class="v"><?php echo $ucache_info['total_hit_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Misses</td>
                    <td class="v"><?php echo $ucache_info['total_miss_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Total memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $ucache_mem_info['memory_total'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Available memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $ucache_mem_info['memory_free'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Memory overhead</td>
                    <td class="v"><?php echo convert_bytes_to_string( $ucache_mem_info['memory_overhead'] ); ?></td>
                </tr>
            </table> 
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( UCACHE_DATA, BAR_CHART, $ucache_info['total_hit_count'], $ucache_info['total_miss_count'] ); ?>
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( UCACHE_DATA, PIE_CHART, $ucache_mem_info['memory_total'] - $ucache_mem_info['memory_free'], $ucache_mem_info['memory_free'] ); ?>
        </div>
        <?php } ?>
    </div>
    <div class="overview">
        <?php if ( $session_cache_available ) {?>
        <div class="leftpanel extra_margin">
            <table style="width: 100%">
                <tr>
                    <th colspan="2">Session Cache Overview</th>
                </tr>
                <tr>
                    <td class="e">Cache scope</td>
                    <td class="v"><?php echo ( isset( $scache_info['is_local_cache'] ) ) ? cache_scope_text( $scache_info['is_local_cache'] ) : 'Unknown'; ?></td>
                </tr>
                <tr>
                    <td class="e">Cache uptime</td>
                    <td class="v"><?php echo ( isset( $scache_info['total_cache_uptime'] ) ) ? seconds_to_words( $scache_info['total_cache_uptime'] ) : 'Unknown'; ?></td>
                </tr>                
                <tr>
                    <td class="e">Cached entries</td>
                    <td class="v"><a href="<?php echo $PHP_SELF, '?page=', SCACHE_DATA, '#filelist'; ?>"><?php echo $scache_info['total_item_count']; ?></a></td>
                </tr>
                <tr>
                    <td class="e">Hits</td>
                    <td class="v"><?php echo $scache_info['total_hit_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Misses</td>
                    <td class="v"><?php echo $scache_info['total_miss_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Total memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $scache_mem_info['memory_total'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Available memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $scache_mem_info['memory_free'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Memory overhead</td>
                    <td class="v"><?php echo convert_bytes_to_string( $scache_mem_info['memory_overhead'] ); ?></td>
                </tr>
            </table> 
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( SCACHE_DATA, BAR_CHART, $scache_info['total_hit_count'], $scache_info['total_miss_count'] ); ?>
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( SCACHE_DATA, PIE_CHART, $scache_mem_info['memory_total'] - $scache_mem_info['memory_free'], $scache_mem_info['memory_free'] ); ?>
        </div>
        <?php } ?>
    </div>    
    <div class="overview">
        <div class="leftpanel">
            <table style="width: 100%">
                <tr>
                    <th colspan="2">Resolve Path Cache Overview</th>
                </tr>
                <tr>
                    <td class="e">Cached entries</td>
                    <td class="v"><a href="<?php echo $PHP_SELF, '?page=', RCACHE_DATA, '#filelist'; ?>"><?php echo $rpcache_file_info['total_file_count']; ?></a></td>
                </tr>
                <tr>
                    <td class="e">Total memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $rpcache_mem_info['memory_total'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Available memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $rpcache_mem_info['memory_free'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Memory overhead</td>
                    <td class="v"><?php echo convert_bytes_to_string( $rpcache_mem_info['memory_overhead'] ); ?></td>
                </tr>
            </table>
        </div>
    </div>
<?php } else if ( $page == FCACHE_DATA ) {
    init_cache_info( FCACHE_DATA );
?>
    <div class="overview">
        <div class="leftpanel extra_margin">
            <table style="width: 100%">
                <tr>
                    <th colspan="2">File Cache Overview</th>
                </tr>
                <tr>
                    <td class="e">Cache uptime</td>
                    <td class="v"><?php echo ( isset( $fcache_file_info['total_cache_uptime'] ) ) ? seconds_to_words( $fcache_file_info['total_cache_uptime'] ) : 'Unknown'; ?></td>
                </tr>
                <tr>
                    <td class="e">Cached files</td>
                    <td class="v"><?php echo $fcache_file_info['total_file_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Total files size</td>
                    <td class="v"><?php echo convert_bytes_to_string( $fcache_summary_info['total_size'] ); ?></td>
                </tr>                    
                <tr>
                    <td class="e">Hits</td>
                    <td class="v"><?php echo $fcache_file_info['total_hit_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Misses</td>
                    <td class="v"><?php echo $fcache_file_info['total_miss_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Total memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $fcache_mem_info['memory_total'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Available memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $fcache_mem_info['memory_free'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Memory overhead</td>
                    <td class="v"><?php echo convert_bytes_to_string( $fcache_mem_info['memory_overhead'] ); ?></td>
                </tr>
            </table>
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( FCACHE_DATA, BAR_CHART, $fcache_file_info['total_hit_count'], $fcache_file_info['total_miss_count'] ); ?>
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( FCACHE_DATA, PIE_CHART, $fcache_mem_info['memory_total'] - $fcache_mem_info['memory_free'], $fcache_mem_info['memory_free'] ); ?>
        </div>
    </div>
    <div class="list" id="filelist">
        <table style="width:100%">
            <tr>
                <th colspan="6">File cache entries</th>
            </tr>
            <tr>
                <th title="Name of the file">File name</th>
                <th title="Size of the file in KB">File size</th>
                <th title="Indicates total amount of time in seconds for which the file has been in the cache">Add time</th>
                <th title="Total amount of time in seconds which has elapsed since the file was last used">Use time</th>
                <th title="Indicates total amount of time in seconds which has elapsed since the file was last checked for file change">Last check</th>
                <th title="Number of times the file has been served from the cache">Hit Count</th>
        </tr>
<?php 
    $sort_key = 'file_name';
    usort( $fcache_file_info['file_entries'], 'cmp' );
    foreach ( $fcache_file_info['file_entries'] as $entry ) {
        echo '<tr title="', $entry['file_name'] ,'">', "\n";
        echo '<td class="e">', get_trimmed_filename( $entry['file_name'], PATH_MAX_LENGTH ),'</td>', "\n";
        echo '<td class="v">', convert_bytes_to_string( $entry['file_size'] ),'</td>', "\n";
        echo '<td class="v">', $entry['add_time'],'</td>', "\n";
        echo '<td class="v">', $entry['use_time'],'</td>', "\n";
        echo '<td class="v">', $entry['last_check'],'</td>', "\n";
        echo '<td class="v">', $entry['hit_count'],'</td>', "\n";
        echo "</tr>\n";
    }
?>
        </table>
    </div>
<?php } else if ( $page == UCACHE_DATA && $ucache_key == null ) {
    if ( $user_cache_available ) { 
        init_cache_info( UCACHE_DATA );
?>
    <div class="overview">
        <div class="leftpanel extra_margin">
            <table style="width: 100%">
                <tr>
                    <th colspan="2">User Cache Overview</th>
                </tr>
                <tr>
                    <td class="e">Cache scope</td>
                    <td class="v"><?php echo ( isset( $ucache_info['is_local_cache'] ) ) ? cache_scope_text( $ucache_info['is_local_cache'] ) : 'Unknown'; ?></td>
                </tr>
                <tr>
                    <td class="e">Cache uptime</td>
                    <td class="v"><?php echo ( isset( $ucache_info['total_cache_uptime'] ) ) ? seconds_to_words( $ucache_info['total_cache_uptime'] ) : 'Unknown'; ?></td>
                </tr>                
                <tr>
                    <td class="e">Cached entries</td>
                    <td class="v"><?php echo $ucache_info['total_item_count'];
                    if ( USE_AUTHENTICATION && $ucache_info['total_item_count'] > 0 ) 
                        echo ' (<a href="', $PHP_SELF, '?page=', UCACHE_DATA, '&amp;clc=1">Clear All</a>)'; ?>
                    </td>
                </tr>
                <tr>
                    <td class="e">Hits</td>
                    <td class="v"><?php echo $ucache_info['total_hit_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Misses</td>
                    <td class="v"><?php echo $ucache_info['total_miss_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Total memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $ucache_mem_info['memory_total'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Available memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $ucache_mem_info['memory_free'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Memory overhead</td>
                    <td class="v"><?php echo convert_bytes_to_string( $ucache_mem_info['memory_overhead'] ); ?></td>
                </tr>
            </table> 
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( UCACHE_DATA, BAR_CHART, $ucache_info['total_hit_count'], $ucache_info['total_miss_count'] ); ?>
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( UCACHE_DATA, PIE_CHART, $ucache_mem_info['memory_total'] - $ucache_mem_info['memory_free'], $ucache_mem_info['memory_free'] ); ?>
        </div>
    </div>
    <div class="list" id="filelist">
        <table style="width:100%">
            <tr>
                <th colspan="6">User cache entries</th>
            </tr>
            <tr>
                <th title="Object Key Name">Key name</th>
                <th title="Type of the object stored">Value type</th>
                <th title="Size of the object stored">Value size</th>
                <th title="Total amount of time in seconds which remains until the object is removed from the cache">Total TTL</th>
                <th title="Total amount of time in seconds which has elapsed since the object was added to the cache">Total age</th>
                <th title="Number of times the object has been served from the cache">Hit Count</th>
        </tr>
<?php
    $count = 0;
    foreach ( $ucache_info['ucache_entries'] as $entry ) {
        echo '<tr title="', $entry['key_name'] ,'">', "\n";
        if ( USE_AUTHENTICATION )
            echo '<td class="e"><a href="', $PHP_SELF, '?page=', UCACHE_DATA, '&key=', urlencode( $entry['key_name'] ), '">', get_trimmed_string( $entry['key_name'], PATH_MAX_LENGTH ),'</a></td>', "\n";
        else
            echo '<td class="e">', get_trimmed_string( $entry['key_name'], PATH_MAX_LENGTH ),'</td>', "\n";
        echo '<td class="v">', $entry['value_type'], '</td>', "\n";
        echo '<td class="v">', convert_bytes_to_string( $entry['value_size']), '</td>', "\n";        
        echo '<td class="v">', $entry['ttl_seconds'],'</td>', "\n";
        echo '<td class="v">', $entry['age_seconds'],'</td>', "\n";
        echo '<td class="v">', $entry['hitcount'],'</td>', "\n";
        echo "</tr>\n";
        if ($count++ > CACHE_MAX_ENTRY && !$show_all_ucache_entries){
            echo '<tr><td colspan="6"><a href="', $PHP_SELF, '?page=', UCACHE_DATA, '&amp;all=1">Show all entries</td></tr>';
            break;
        }
    }
?>
        </table>
    </div>
<?php } else { ?>
    <div class="overview">
        <p class="notice">The user cache is not available. Enable the user cache by using <strong>wincache.ucenabled</strong> 
            directive in <strong>php.ini</strong> file.</p>
    </div>
<?php }?>
<?php } else if ( $page == UCACHE_DATA && $ucache_key != null && USE_AUTHENTICATION ) {
            if ( !wincache_ucache_exists( $ucache_key ) ){
?>
    <div class="overview">
        <p class="notice">The variable with this key does not exist in the user cache.</p>
    </div>
<?php       } 
            else{
                $ucache_entry_info = wincache_ucache_info( true, $ucache_key );
?>
    <div class="list">
        <table width="60%">
            <tr>
                <th colspan="2">User Cache Entry Information</th>
            </tr>
            <tr>
                <td class="e">Key</td>
                <td class="v"><?php echo $ucache_entry_info['ucache_entries'][1]['key_name']; ?></td>
            </tr>
            <tr>
                <td class="e">Value Type</td>
                <td class="v"><?php echo $ucache_entry_info['ucache_entries'][1]['value_type']; ?></td>
            </tr>
            <tr>
                <td class="e">Size</td>
                <td class="v"><?php echo convert_bytes_to_string( $ucache_entry_info['ucache_entries'][1]['value_size'] ); ?></td>
            </tr>
            <tr>
                <td class="e">Total Time To Live (in seconds)</td>
                <td class="v"><?php echo $ucache_entry_info['ucache_entries'][1]['ttl_seconds']; ?></td>
            </tr>
            <tr>
                <td class="e">Total Age (in seconds)</td>
                <td class="v"><?php echo $ucache_entry_info['ucache_entries'][1]['age_seconds']; ?></td>
            </tr>
            <tr>
                <td class="e">Hit Count</td>
                <td class="v"><?php echo $ucache_entry_info['ucache_entries'][1]['hitcount']; ?></td>
            </tr>
        </table>
    </div>
    <div id="panel">
        <div id="panel_header">
            User Cache Entry Content
        </div>
        <div id="panel_body">
            <pre><?php var_dump( wincache_ucache_get( $ucache_key ) )?></pre>
        </div>
    </div>
<?php }?>
<?php } else if ( $page == SCACHE_DATA ) {
    if ( $session_cache_available ) {
        init_cache_info( SCACHE_DATA );
?>
    <div class="overview">
        <div class="leftpanel extra_margin">
            <table style="width: 100%">
                <tr>
                    <th colspan="2">Session Cache Overview</th>
                </tr>
                <tr>
                    <td class="e">Cache scope</td>
                    <td class="v"><?php echo ( isset( $scache_info['is_local_cache'] ) ) ? cache_scope_text( $scache_info['is_local_cache'] ) : 'Unknown'; ?></td>
                </tr>
                <tr>
                    <td class="e">Cache uptime</td>
                    <td class="v"><?php echo ( isset( $scache_info['total_cache_uptime'] ) ) ? seconds_to_words( $scache_info['total_cache_uptime'] ) : 'Unknown'; ?></td>
                </tr>                
                <tr>
                    <td class="e">Cached entries</td>
                    <td class="v"><?php echo $scache_info['total_item_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Hits</td>
                    <td class="v"><?php echo $scache_info['total_hit_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Misses</td>
                    <td class="v"><?php echo $scache_info['total_miss_count']; ?></td>
                </tr>
                <tr>
                    <td class="e">Total memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $scache_mem_info['memory_total'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Available memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $scache_mem_info['memory_free'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Memory overhead</td>
                    <td class="v"><?php echo convert_bytes_to_string( $scache_mem_info['memory_overhead'] ); ?></td>
                </tr>
            </table> 
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( SCACHE_DATA, BAR_CHART, $scache_info['total_hit_count'], $scache_info['total_miss_count'] ); ?>
        </div>
        <div class="rightpanel">
            <?php echo get_chart_markup( SCACHE_DATA, PIE_CHART, $scache_mem_info['memory_total'] - $scache_mem_info['memory_free'], $scache_mem_info['memory_free'] ); ?>
        </div>
    </div>
    <div class="list" id="sessionlist">
        <table style="width:100%">
            <tr>
                <th colspan="6">Session cache entries</th>
            </tr>
            <tr>
                <th title="Object Key Name">Key name</th>
                <th title="Type of the object stored">Value type</th>
                <th title="Size of the object stored">Value size</th>
                <th title="Total amount of time in seconds which remains until the object is removed from the cache">Total TTL</th>
                <th title="Total amount of time in seconds which has elapsed since the object was added to the cache">Total age</th>
                <th title="Number of times the object has been served from the cache">Hit Count</th>
        </tr>
<?php 
    $count = 0;
    foreach ( $scache_info['scache_entries'] as $entry ) {
        echo '<tr title="', $entry['key_name'] ,'">', "\n";
        echo '<td class="e">', get_trimmed_string( $entry['key_name'], PATH_MAX_LENGTH ),'</td>', "\n";
        echo '<td class="v">', $entry['value_type'], '</td>', "\n";
        echo '<td class="v">', convert_bytes_to_string( $entry['value_size'] ), '</td>', "\n";
        echo '<td class="v">', $entry['ttl_seconds'],'</td>', "\n";
        echo '<td class="v">', $entry['age_seconds'],'</td>', "\n";
        echo '<td class="v">', $entry['hitcount'],'</td>', "\n";
        echo "</tr>\n";
        if ($count++ > CACHE_MAX_ENTRY && !$show_all_ucache_entries){
            echo '<tr><td colspan="6"><a href="', $PHP_SELF, '?page=', SCACHE_DATA, '&amp;all=1">Show all entries</td></tr>';
            break;
        }
    }
?>
        </table>
    </div>
<?php } else { ?>
    <div class="overview">
        <p class="notice">The session cache is not enabled. To enable session cache set the session handler in <strong>php.ini</strong> to 
        <strong>wincache</strong>, for example: <strong>session.save_handler=wincache</strong>.</p>
    </div>
<?php }?>
<?php } else if ( $page == RCACHE_DATA ) {
    init_cache_info( RCACHE_DATA );
?>
    <div class="overview">
        <div class="wideleftpanel">
            <table style="width: 100%">
                <tr>
                    <th colspan="2">Resolve Path Cache Overview</th>
                </tr>
                <tr>
                    <td class="e">Cached entries</td>
                    <td class="v"><?php echo $rpcache_file_info['total_file_count'] ?></td>
                </tr>
                <tr>
                    <td class="e">Total memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $rpcache_mem_info['memory_total'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Available memory</td>
                    <td class="v"><?php echo convert_bytes_to_string( $rpcache_mem_info['memory_free'] ); ?></td>
                </tr>
                <tr>
                    <td class="e">Memory overhead</td>
                    <td class="v"><?php echo convert_bytes_to_string( $rpcache_mem_info['memory_overhead'] ); ?></td>
                </tr>
            </table>
        </div>
    </div>
    <div class="list" id="filelist">
        <table style="width:100%">
            <tr>
                <th colspan="2">Resolve path cache entries</th>
            </tr>
            <tr>
                <th>Resolve path</th>
                <th>Subkey data</th>
        </tr>
<?php 
    $sort_key = 'resolve_path';
    usort( $rpcache_file_info['rplist_entries'], 'cmp' );
    foreach ( $rpcache_file_info['rplist_entries'] as $entry ) {
        echo '<tr title="',$entry['subkey_data'], '">', "\n";
        echo '<td class="e">', get_trimmed_string( $entry['resolve_path'], PATH_MAX_LENGTH ),'</td>', "\n";
        echo '<td class="v">', get_trimmed_string( $entry['subkey_data'], SUBKEY_MAX_LENGTH ), '</td>', "\n";
        echo "</tr>\n";
    }
?>
        </table>
    </div>
<?php } ?>
<div class="clear"></div>
</div>
</body>

</html>
