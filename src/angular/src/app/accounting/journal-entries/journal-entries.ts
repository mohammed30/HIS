import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { JournalEntryService } from '../../proxy/accounting/journal-entry.service';
import { JournalEntryDto } from '../../proxy/accounting/models';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { CoreModule } from '@abp/ng.core';

@Component({
  selector: 'app-journal-entries',
  standalone: true,
  imports: [CommonModule, NgbModule, NgxDatatableModule, CoreModule],
  templateUrl: './journal-entries.html',
  styleUrls: ['./journal-entries.scss'],
  providers: [ListService],
})
export class JournalEntriesComponent implements OnInit {
  journalEntries = { items: [], totalCount: 0 } as PagedResultDto<JournalEntryDto>;

  constructor(public readonly list: ListService, private journalEntryService: JournalEntryService) { }

  ngOnInit() {
    this.list.hookToQuery((query) => this.journalEntryService.getList(query)).subscribe((response) => {
      this.journalEntries = response;
    });
  }
}
