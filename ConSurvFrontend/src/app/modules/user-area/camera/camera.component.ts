import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CameraDTO, CameraService } from '../../../generated/con-surv-backend';
import { Observable, of, switchMap } from 'rxjs';
import { StorageService } from '../../../services/storage.service';

@Component({
  selector: 'app-camera',
  standalone: false,
  templateUrl: './camera.component.html',
  styleUrl: './camera.component.scss'
})
export class CameraComponent implements OnInit {

  currentCamera$: Observable<CameraDTO | null>;


  constructor(private route: ActivatedRoute, private cameraService: CameraService, private storgeService: StorageService) {
    this.currentCamera$ = this.route.queryParams.pipe(switchMap(params => {
      if (params["cameraId"]) {
        return this.cameraService.aPIV1CameraControllerCameraCameraIdGet(params["cameraId"], this.storgeService.getAccessToken());
      } else {
        return of(null);
      }
    }));
  }
  /*
  processing: boolean = false;
  queue: ArrayBuffer[] = [];
  */
  private mediaSource: MediaSource | null = null;
  private sourceBuffer: SourceBuffer | null = null;
  private queue: Uint8Array[] = [];
  private socket: WebSocket | null = null;
  private processing: boolean = false;

  private sourceBufferReady = true;
  ngOnInit(): void {

    this.currentCamera$.subscribe((currentCamera) => {
      try {
        //see https://stackoverflow.com/questions/4241992/video-streaming-over-websockets-using-javascript
        // or https://stackoverflow.com/questions/16486930/using-websocket-to-stream-in-video-tag
        // or https://stackoverflow.com/questions/8980858/streaming-video-to-web-browser





        const mediaSource = new MediaSource();
        const video = document.getElementById('videoElement') as HTMLVideoElement;
        video.src = URL.createObjectURL(mediaSource);

        mediaSource.addEventListener("sourceopen", () => {
          //const sourceBuffer = mediaSource.addSourceBuffer('video/mp4; codecs="avc1.640028, mp4a.40.2"');
          this. sourceBuffer = mediaSource.addSourceBuffer('video/mp2t; codecs="avc1.640028, mp4a.40.2"');
          const ws = new WebSocket("wss://consurv.test.local:443/API/ws2/wsStream");//C:\Temp\x.m3u8

          ws.onopen = () => {
            console.log('WebSocket verbunden');
          }
          ws.binaryType = "arraybuffer";
          ws.addEventListener("message", (event) => {
            console.log('message+'+event.data.byteLength.toString());
            //console.log(event);
            this.sourceBuffer!.appendBuffer(new Uint8Array(event.data));
          });

          this.sourceBuffer!.addEventListener("updateend", () => {
            this.sourceBufferReady = true;
            if (this.queue.length > 0) {
              this.processQueue();
            }
        });
        });

        /*
        this.mediaSource = new MediaSource();
        this.socket = new WebSocket('wss://consurv.test.local:443/API/ws2/wsStream');
        this.socket.binaryType = 'arraybuffer';
        this.socket.onopen = () => {
          console.log('WebSocket verbunden');
          const video = document.getElementById('videoElement') as HTMLVideoElement;

          video.src = URL.createObjectURL(this.mediaSource!);

          this.mediaSource!.addEventListener('sourceopen', () => {
            //this.sourceBuffer.addEventListener('updateend', () => this.appendNext());
            console.log("onsourceopen");



            this.sourceBuffer = this.mediaSource!.addSourceBuffer("video/mp4; codecs=\"avc1.640028, mp4a.40.2\"");
            this.socket!.onmessage = (event) => {
              const data = new Uint8Array(event.data);
              console.log("onmessage");
              this.queue.push(data);
              if (!this.sourceBuffer || this.sourceBuffer.updating || !this.queue.length) {
                console.log("Skipping processQueue: sourceBuffer missing or busy.");
                return;
              }
              console.log("append");
              this.sourceBuffer.appendBuffer(this.queue.shift()!);
            };

            //   this.socket.onclose = () => console.log('WebSocket geschlossen');

          });
        };
*/

        /*
                if (Hls.isSupported()) {
                  var video = document.getElementById('videoElement');
                  var hls = new Hls();
                  hls.loadSource('http://dein-server/stream.m3u8');
                  hls.attachMedia(video);
                }
                  */
        /*
        const video = document.getElementById("videoElement") as HTMLVideoElement;
        const mediaSource = new MediaSource();
        console.log("Created MediaSource:", mediaSource);

        console.log("Found video element:", video);
        video.src = URL.createObjectURL(mediaSource);

        mediaSource.addEventListener("sourceopen", () => {
          console.log("MediaSource opened");
          const webSocket = new WebSocket('wss://consurv.test.local:443/API/ws2/wsStream');

          webSocket.onopen = function () {
            console.log('WebSocket verbunden');
          };
          this.sourceBuffer = mediaSource.addSourceBuffer("video/mp4; codecs=\"avc1.640028, mp4a.40.2\"");
          this.sourceBuffer.addEventListener("updateend", () => {
            console.log("updateend triggered");
            this.processing = false;
            if (this.queue.length == 0) {
              this.processQueue();
            }
          });

          webSocket.binaryType = "arraybuffer";

          webSocket.addEventListener("message", (event) => {
            console.log("queue push")
            console.log("queue count: " + this.queue.length.toString())
            this.queue.push(event.data);
            if (!this.processing) {
              this.processQueue();
            }
          });
        });
*/
        /*
        mediaSource.addEventListener("sourceopen", () => {
          const sourceBuffer = mediaSource.addSourceBuffer('video/mp4; codecs="avc1.64001f, mp4a.40.2"');
          console.log("sourceopen");
          webSocket.onmessage = (event) => {
            console.log("onmessage");
            sourceBuffer.appendBuffer(new Uint8Array(event.data));
          };
        });
        */

        /*
        mediaSource.onsourceopen = () => {
          console.log("onsourceopen");
          let buffer = mediaSource.addSourceBuffer('video/webm; codecs="vorbis,vp8"');
          webSocket.onmessage = (event) => {
            // event.data is a blob
            buffer.appendBuffer(new Uint8Array(event.data));
            console.log("onmessage");
          };
        };
        */
      } catch (error) {
        console.error(error);
      }
    });
  }

   processQueue() :void{
    if (this.sourceBufferReady && this.queue.length > 0) {
      this.sourceBufferReady = false;
      this. sourceBuffer!.appendBuffer(this.queue!.shift()!);
    }
}

  processQueue2(): void {
    if (!this.sourceBuffer || !this.queue.length || this.sourceBuffer.updating) {
      console.log("Skipping processQueue: sourceBuffer missing or busy.");
      return;
    }
    console.log("processing queue");

    this.processing = true;
    const data = this.queue.shift();
    if (data) {
      this.sourceBuffer.appendBuffer(new Uint8Array(data));
    }
  }

}
