import { Component, AfterViewInit, OnDestroy, inject, PLATFORM_ID, ElementRef, Renderer2 } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { CoreModule } from '@abp/ng.core';

@Component({
    selector: 'app-sidebar-search',
    standalone: true,
    imports: [CommonModule, CoreModule],
    template: `
        <div #searchContainer class="sidebar-search-container" style="display: none;">
            <div class="sidebar-search-box">
                <i class="fas fa-search search-icon"></i>
                <input type="text" [placeholder]="'::SearchInMenu' | abpLocalization" (input)="onSearch($event)" />
            </div>
        </div>
    `,
    styles: [`
        .sidebar-search-container {
            padding: 10px 15px;
            background-color: #111827;
            position: sticky;
            top: 75px; /* Adjust based on logo height */
            z-index: 1010;
            border-bottom: 1px solid #1f2937;
        }
        .sidebar-search-box {
            position: relative;
            display: flex;
            align-items: center;
        }
        .sidebar-search-box .search-icon {
            position: absolute;
            right: 12px;
            color: #6b7280;
            font-size: 0.9rem;
        }
        .sidebar-search-box input {
            width: 100%;
            background-color: #1f2937;
            border: 1px solid #374151;
            border-radius: 8px;
            color: #e5e7eb;
            padding: 8px 35px 8px 12px; /* right padding for icon since RTL */
            font-size: 0.85rem;
            outline: none;
            transition: border-color 0.2s, box-shadow 0.2s;
        }
        .sidebar-search-box input:focus {
            border-color: #3b82f6;
            box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.2);
        }
        .sidebar-search-box input::placeholder {
            color: #6b7280;
        }
        
        [dir="ltr"] .sidebar-search-box .search-icon {
            right: auto;
            left: 12px;
        }
        [dir="ltr"] .sidebar-search-box input {
            padding: 8px 12px 8px 35px;
        }
    `]
})
export class SidebarSearchComponent implements AfterViewInit, OnDestroy {
    private platformId = inject(PLATFORM_ID);
    private el = inject(ElementRef);
    private renderer = inject(Renderer2);
    
    private checkInterval: any;
    private injected = false;

    private searchBoxNode: HTMLElement | null = null;

    ngAfterViewInit() {
        if (isPlatformBrowser(this.platformId)) {
            const originalSearchBox = this.el.nativeElement.querySelector('.sidebar-search-container');
            if (originalSearchBox) {
                this.searchBoxNode = originalSearchBox;
            }

            // Continuously check for the sidebar logo container so we can insert the search box below it
            this.checkInterval = setInterval(() => {
                const logoContainer = document.querySelector('.lpx-sidebar .lpx-logo-container');
                
                if (logoContainer && this.searchBoxNode) {
                    const parent = logoContainer.parentElement;
                    const existingInSidebar = parent?.querySelector('.sidebar-search-container');
                    
                    if (!existingInSidebar && parent) {
                        const searchBox = this.searchBoxNode;
                        searchBox.style.display = 'block';
                        
                        // Bind native event listener because moving DOM element breaks Angular bindings
                        const inputEl = searchBox.querySelector('input');
                        if (inputEl) {
                            // Clone to remove previous event listeners and prevent duplicates
                            const newInput = inputEl.cloneNode(true) as HTMLInputElement;
                            inputEl.parentNode?.replaceChild(newInput, inputEl);
                            newInput.addEventListener('input', (event: any) => this.onSearch(event));
                            newInput.addEventListener('keyup', (event: any) => this.onSearch(event));
                        }

                        // Insert search box right after the logo container
                        this.renderer.insertBefore(parent, searchBox, logoContainer.nextSibling);
                        this.injected = true;
                    }
                }
            }, 1000);
        }
    }

    normalizeArabic(text: string): string {
        if (!text) return '';
        return text.replace(/[أإآ]/g, 'ا')
                   .replace(/ة/g, 'ه')
                   .replace(/ي/g, 'ى');
    }

    clearHighlights() {
        document.querySelectorAll('.search-highlight').forEach(el => {
            const parent = el.parentNode;
            if (parent) {
                parent.replaceChild(document.createTextNode(el.textContent || ''), el);
                parent.normalize();
            }
        });
    }

    highlightText(element: Element, searchTerm: string) {
        if (!element || !searchTerm) return;
        const walker = document.createTreeWalker(element, NodeFilter.SHOW_TEXT, null);
        let node;
        const nodesToReplace = [];
        while ((node = walker.nextNode())) {
            if (node.nodeValue && this.normalizeArabic(node.nodeValue.toLowerCase()).includes(searchTerm)) {
                nodesToReplace.push(node);
            }
        }
        
        nodesToReplace.forEach(n => {
            const val = n.nodeValue || '';
            const normalized = this.normalizeArabic(val.toLowerCase());
            const idx = normalized.indexOf(searchTerm);
            if (idx >= 0) {
                const before = val.substring(0, idx);
                // We use the original string length for the match because normalization might change length (though our current one doesn't).
                // A safer way is to just grab the exact substring from original based on the normalized index.
                const match = val.substring(idx, idx + searchTerm.length);
                const after = val.substring(idx + searchTerm.length);
                
                const span = document.createElement('span');
                span.className = 'search-highlight';
                span.style.backgroundColor = 'rgba(255, 193, 7, 0.4)'; // subtle yellow
                span.style.color = '#fff';
                span.style.borderRadius = '2px';
                span.textContent = match;
                
                const parent = n.parentNode;
                if (parent) {
                    if (before) parent.insertBefore(document.createTextNode(before), n);
                    parent.insertBefore(span, n);
                    if (after) parent.insertBefore(document.createTextNode(after), n);
                    parent.removeChild(n);
                }
            }
        });
    }

    onSearch(event: any) {
        const rawTerm = event.target.value.toLowerCase().trim();
        const searchTerm = this.normalizeArabic(rawTerm);
        
        this.clearHighlights();
        
        // LeptonX might use different wrappers, so search globally for menu items
        const menuItems = document.querySelectorAll('.lpx-menu-item, .lpx-nav-item, li, .abp-menu-item');
        
        menuItems.forEach((item: any) => {
            const rawText = item.textContent?.toLowerCase() || '';
            const textContent = this.normalizeArabic(rawText);
            const textContainer = item.querySelector('.lpx-menu-item-text, .lpx-nav-item-text, span');
            
            // If empty search, show everything and reset states
            if (!searchTerm) {
                item.style.setProperty('display', '', 'important');
                item.classList.remove('open', 'active', 'show'); // Let the framework handle it naturally
                const innerMenu = item.querySelector('.lpx-inner-menu, ul');
                if (innerMenu) {
                    innerMenu.style.removeProperty('display');
                    innerMenu.style.removeProperty('height');
                    innerMenu.style.removeProperty('max-height');
                    innerMenu.style.removeProperty('opacity');
                    innerMenu.style.removeProperty('visibility');
                }
                return;
            }
            
            // Basic filtering logic
            if (textContent.includes(searchTerm)) {
                item.style.setProperty('display', '', 'important');
                
                // Highlight text if this is not just a parent container
                if (textContainer) {
                    this.highlightText(textContainer, searchTerm);
                } else if (item.children.length === 0 || (item.children.length === 1 && item.children[0].tagName === 'A')) {
                    this.highlightText(item, searchTerm);
                }
                
                // Also show parents if this is a nested item
                let parent = item.closest('.lpx-inner-menu, .lpx-nav-menu, ul');
                while (parent) {
                    // Force the inner menu itself to be visible
                    if (parent.classList.contains('lpx-inner-menu') || parent.tagName === 'UL') {
                        parent.style.setProperty('display', 'block', 'important');
                        parent.style.setProperty('height', 'auto', 'important');
                        parent.style.setProperty('max-height', 'none', 'important');
                        parent.style.setProperty('opacity', '1', 'important');
                        parent.style.setProperty('visibility', 'visible', 'important');
                    }
                    
                    const parentMenuItem = parent.parentElement?.closest('.lpx-menu-item, .lpx-nav-item, li, .abp-menu-item');
                    if (parentMenuItem) {
                        parentMenuItem.style.setProperty('display', '', 'important');
                        parentMenuItem.classList.add('open', 'active', 'show'); // Try multiple classes used by frameworks
                    }
                    parent = parentMenuItem ? parentMenuItem.parentElement?.closest('.lpx-inner-menu, .lpx-nav-menu, ul') : null;
                }
            } else {
                // Hide if doesn't match and doesn't contain matching children
                const hasMatchingChildren = Array.from(item.querySelectorAll('.lpx-menu-item, .lpx-nav-item, li, .abp-menu-item')).some(
                    (child: any) => {
                        const childText = child.textContent?.toLowerCase() || '';
                        return this.normalizeArabic(childText).includes(searchTerm);
                    }
                );
                
                if (!hasMatchingChildren) {
                    item.style.setProperty('display', 'none', 'important');
                    item.classList.remove('open', 'active', 'show');
                    
                    // Also hide inner menus
                    const innerMenu = item.querySelector('.lpx-inner-menu, ul');
                    if (innerMenu) {
                        innerMenu.style.setProperty('display', 'none', 'important');
                    }
                } else {
                    item.style.setProperty('display', '', 'important');
                    item.classList.add('open', 'active', 'show');
                    
                    // Show inner menus if children match
                    const innerMenu = item.querySelector('.lpx-inner-menu, ul');
                    if (innerMenu) {
                        innerMenu.style.setProperty('display', 'block', 'important');
                        innerMenu.style.setProperty('height', 'auto', 'important');
                        innerMenu.style.setProperty('max-height', 'none', 'important');
                        innerMenu.style.setProperty('opacity', '1', 'important');
                        innerMenu.style.setProperty('visibility', 'visible', 'important');
                    }
                }
            }
        });
    }

    ngOnDestroy() {
        if (this.checkInterval) {
            clearInterval(this.checkInterval);
        }
        
        // Remove search container if it was moved
        if (this.injected && isPlatformBrowser(this.platformId)) {
            const searchBox = document.querySelector('.sidebar-search-container');
            if (searchBox) {
                searchBox.remove();
            }
        }
    }
}
