const {merge} = require('webpack-merge');
const common = require('./webpack.config.js');
const CssMinimizerPlugin  = require('css-minimizer-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');

module.exports = merge(common, {
    mode: 'production',
    devtool: 'source-map',

    optimization: {
        minimize: true,
        minimizer: [
            new CssMinimizerPlugin( {
                parallel: true,
            }),
            new TerserPlugin({
                // Use multi-process parallel running to improve the build speed
                // Default number of concurrent runs: os.cpus().length - 1
                parallel: true,
            }),
        ],
    }
});