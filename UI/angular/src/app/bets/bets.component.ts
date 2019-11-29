import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';

import { environment } from '../../environments/environment';
import { Match, RoundInfo, Bet, RoundBets } from '../_models';
import { SnookerFeedService, BetsService } from '../_services';

@Component({
  selector: 'app-bets',
  templateUrl: './bets.component.html',
  styleUrls: ['./bets.component.less']
})
export class BetsComponent implements OnInit {
  private roundId = environment.roundId;

  private bets: Bet[];
  private roundInfo: RoundInfo;

  loading = false;

  constructor(private betsService: BetsService,
              private snookerFeedService: SnookerFeedService) { }

  ngOnInit() {
    this.loading = true;

    const roundInfoRequest = this.snookerFeedService.getRoundInfo(this.roundId);
    const betsRequest = this.betsService.getBets(this.roundId);

    forkJoin([roundInfoRequest, betsRequest]).subscribe(results => {
      this.roundInfo = results[0];
      this.bets = results[1].matchBets;

      this.loading = false;
    }, error => console.log(error));
  }

  submit() {
    const roundBets = new RoundBets({ roundId: this.roundId, matchBets: this.bets });
    this.betsService.submitBets(roundBets).subscribe();
  }
}
