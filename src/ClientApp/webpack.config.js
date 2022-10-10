const path = require('path')
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const devMode = process.env.NODE_ENV === 'development';

module.exports = {
    resolve: {
        fallback: {
            "url":false
        }
    },
    entry: {
        javascript: "./app/main.js",
    },
    output: {
        path: path.resolve(__dirname, 'public'),
        publicPath: '/',
        filename: 'bundle.js'
    },
    module: {
        rules: [
        {
            test: /\.riot$/,
            exclude: /node_modules/,
            use: [{
                loader: '@riotjs/webpack-loader',
                options: {
                    hot: devMode,
                }
            }]
        },

        {
            test: /\.(sass|scss|css)$/,
            use: [{
                loader: (devMode) ? "style-loader": MiniCssExtractPlugin.loader
            },
                { loader: "css-loader", options: { sourceMap: true } },
                { loader: "sass-loader", options: { sourceMap: true } },
            ],
        },
        {
            test: /\.(png|jpg|gif|svg|eot|woff|woff2|ttf)$/,
            use: [{
                loader: 'file-loader',
                options: {
                    name: '[name].[ext]',
                    outputPath: './assets/',
                }
            }]
        }
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            // Options similar to the same options in webpackOptions.output
            // all options are optional
            filename: '[name].css',
            chunkFilename: '[id].css',
            ignoreOrder: false, // Enable to remove warnings about conflicting order
        }),
        new HtmlWebpackPlugin({
            filename: 'index.html',
            inject: true,
            template: path.resolve(__dirname, 'app', 'index.html'),
        })
    ],
}