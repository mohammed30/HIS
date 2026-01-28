import { Component, OnInit } from '@angular/core';
// Item Card Component
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { RestService } from '@abp/ng.core';
import { InventoryItemDto, InventoryTransactionDto, TransactionType } from '../../proxy/inventory/models';
import { LocalizationModule } from '@abp/ng.core';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

@Component({
  selector: 'app-item-card',
  standalone: true,
  imports: [CommonModule, LocalizationModule, NgxDatatableModule],
  templateUrl: './item-card.component.html',
  styleUrls: ['./item-card.scss']
})
export class ItemCardComponent implements OnInit {
  itemId: string;
  item: InventoryItemDto | null = null;
  transactions: InventoryTransactionDto[] = [];
  transactionType = TransactionType;

  constructor(
    private route: ActivatedRoute,
    private restService: RestService
  ) { }

  ngOnInit() {
    this.itemId = this.route.snapshot.params['id'];
    this.loadItemDetails();
    this.loadTransactions();
  }

  loadItemDetails() {
    this.restService.request<void, InventoryItemDto>({
      method: 'GET',
      url: `/api/app/inventory/item/${this.itemId}`
    }).subscribe(res => {
      this.item = res;
    });
  }

  loadTransactions() {
    this.restService.request<void, InventoryTransactionDto[]>({
      method: 'GET',
      url: `/api/app/inventory/item-transactions/${this.itemId}`
    }).subscribe(res => {
      this.transactions = res;
    });
  }
}
