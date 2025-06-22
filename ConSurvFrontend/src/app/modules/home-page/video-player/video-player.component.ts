import { Component, ElementRef, Input, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import videojs from 'video.js';
import Player from 'video.js/dist/types/player';

@Component({
  selector: 'app-video-player',
  standalone: false,
  templateUrl: './video-player.component.html',
  styleUrl: './video-player.component.scss'
})
export class VideoPlayerComponent implements OnInit, OnDestroy {
  // @ViewChild('target', { static: true }) target: ElementRef | null = null;
  @Input() options: {
    fluid: boolean,
    aspectRatio: string,
    autoplay: boolean,
    sources: {
      src: string,
      type: string,
    }[],
  } | null = null;

  player: Player | null = null;

  constructor(
    //  private elementRef: ElementRef,
  ) {
    // this.target = this.elementRef;
  }

  ngOnInit() {
    console.log("config");
    console.log(this.options);
    this.player = videojs("videoPlayer", this.options, function onPlayerReady() {
      console.log('onPlayerReady', this);
    });
  }

  ngOnDestroy() {
    if (this.player) {
      this.player.dispose();
    }
  }
}
