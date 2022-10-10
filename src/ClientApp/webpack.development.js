const {merge} = require('webpack-merge');
const common = require('./webpack.config.js');
const path = require('path');

module.exports = merge(common, {
    mode: 'development',
    devtool: 'inline-source-map',
    devServer: {
        open: false,
        client:  {
            logging: 'info',
            progress: false
        },
        compress: true,
        host: '0.0.0.0',
        port: process.env.DEV_SERVER_PORT,
        hot: true,
        devMiddleware: {
            publicPath: '/',
            writeToDisk: true,
        }, 
        watchFiles: {
            paths: ['src/**/*', 'public/**/*'],
            options: {
              usePolling: true,
            },
        },        
        proxy: {
            '/api': {
                pathRewrite: { '^/api': '' },
                secure: false,
                changeOrigin: true
            }
        },
        historyApiFallback: true
    }

});