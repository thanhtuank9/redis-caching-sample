# Redis cache Install on Window server

#### 1. Download and Install Redis on window server
##### - Link: https://github.com/ServiceStack/redis-windows/raw/master/downloads/redis-latest.zip
##### - Extract to: C:\redis-latest (Recommened)
##### - Go into that folder and run command line: "*redis-server --service-install redis.windows.conf --loglevel verbose*"
##### - Continue to start service: "*redis-server --service-start*"
##### - Please check Redis in the Service to make sure it works, like [this image](http://i.imgur.com/v07MKdw.png "this")

#### 2. UI Tool
##### - Get and install at: [https://redisdesktop.com/](https://redisdesktop.com/)
##### - Default connection:
1. Host: locahost
1. Port: 6379
1. Pass: empty is default
