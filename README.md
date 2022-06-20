# Meeting

Meeting is a demonstration project of my skills and knowledge in the field of Xaml platforms.
At the moment, it allows you to arrange group video meetings.

### Technology stack

- WPF
- Xamarin
- Grpc

### Server part

As default UI platforms use [Amazon Web Services]([url](https://aws.amazon.com/)) to maintain a free instance and wors in 3.72.127.66.
If you want to use server as localy change connectin method:
```C#
        private string GetServerAddress()
        {
            var address = "https://3.72.127.66:5010";
            if (false) // Hot switcher
            {
                address = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:5010" : "https://localhost:5010";
            }
            return address;
        }
```

You can read more about the server part there:

https://github.com/LimeniyeCyberforum/meeting-backend 

### Preview



https://user-images.githubusercontent.com/69314237/174568307-6edc5b5b-9a48-4a73-aafb-ad35f26715ba.mp4



https://user-images.githubusercontent.com/69314237/173517147-fdca5fc2-ee4f-4d33-babc-a33b3a94314a.mp4

