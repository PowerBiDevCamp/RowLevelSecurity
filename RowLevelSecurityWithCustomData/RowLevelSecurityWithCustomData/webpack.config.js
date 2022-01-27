const path = require('path');

module.exports = {
  entry: './App/app.ts',
  output: {
    filename: 'app.js',
    path: path.resolve(__dirname, 'wwwroot/js'),
  },
  resolve: {
    extensions: ['.js', '.ts']
  },
  module: {
    rules: [
      { test: /\.(ts)$/, loader: 'awesome-typescript-loader' }
    ],
  },
  mode: "development",
  devtool: 'source-map'
};
