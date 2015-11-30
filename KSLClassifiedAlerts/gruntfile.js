/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    grunt.initConfig({
        bower: {
            install: {
                options: {
                    targetDir: "client/lib",
                    layout: "byComponent",
                    cleanTargetDir: false
                }
            }
        },
    });

    grunt.registerTask("install-bower-packs", ["bower:install"]);

    grunt.loadNpmTasks("grunt-bower-task");

};