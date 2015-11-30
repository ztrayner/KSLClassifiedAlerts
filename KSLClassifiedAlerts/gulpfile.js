/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp'),
    jshint = require('gulp-jshint'),
    del = require('del'),
    less = require('gulp-less'),
    autoprefixer = require('gulp-autoprefixer')
    minifyCSS = require('gulp-minify-css')
    rename = require('gulp-rename')
    ngAnnotate = require('gulp-ng-annotate'),
    stylish = require('jshint-stylish'),
    uglify = require('gulp-uglify'),
    sourcemaps = require('gulp-sourcemaps'),
    concat = require('gulp-concat');

var paths = {
    homeApp: "./client/app/home/**/*.js",
    homeAppDist: "./client/dist/home/",
    src: "./client/app/**/*.js",
    dest: "./client/dist/",
    styles: "./client/styles/",
    less: "./client/styles/Site.less"
}

gulp.task("del", function () {
    del(paths.dest + '**/*');    // Delete everything in dest
});

gulp.task('home', ['clean'], function () {
    return gulp.src(paths.homeApp)         // Returns a stream
        .pipe(jshint())
        .pipe(jshint.reporter('default'))
        .pipe(gulp.dest(paths.homeAppDist))   // Pipes the stream somewhere
});
gulp.task('less2css', function () {
    gulp.src(paths.less)
       .pipe(less())
       .pipe(gulp.dest(paths.styles))
       .pipe(minifyCSS({ keepBreaks: false }))
       .pipe(rename({ suffix: '.min' }))
       .pipe(gulp.dest(paths.styles));
});

gulp.task('ngAnnotate', function () {
    return gulp.src(paths.homeApp)
        .pipe(ngAnnotate())
        .pipe(gulp.dest(paths.homeAppDist));
});

gulp.task('js', ['jshint'], function () {
    var source = paths.js;

    return gulp.src(source)
        .pipe(sourcemaps.init())
        .pipe(concat('homeApp.min.js', { newLine: ';' }))
        // Annotate before uglify so the code get's min'd properly.
        .pipe(ngAnnotate({
            // true helps add where @ngInject is not used. It infers.
            // Doesn't work with resolve, so we must be explicit there
            add: true
        }))
        .pipe(bytediff.start())
        .pipe(uglify({ mangle: true }))
        .pipe(bytediff.stop())
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest(paths.dev));
});

//  Hints and builds all JavaScript.
gulp.task('js', function () {
    return gulp.src([paths.homeApp])
      .pipe(jshint())
      .pipe(jshint.reporter(stylish))
      .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat('homeApp.js'))
        .pipe(gulp.dest(paths.homeAppDist))
        .pipe(ngAnnotate())
        .pipe(uglify())
        .pipe(rename({ suffix: '.min' }))
      .pipe(sourcemaps.write('./'))
      .pipe(gulp.dest(paths.homeAppDist));
});