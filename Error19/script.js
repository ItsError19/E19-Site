/**
 * Main application script
 * Handles dark mode toggle, terminal copy functionality, and Terminator effects
 */

document.addEventListener('DOMContentLoaded', function() {
    // Initialize dark mode functionality
    initDarkMode();
    
    // Initialize terminal copy functionality if terminal exists
    const terminal = document.querySelector('.terminal');
    if (terminal) {
        initTerminalCopy();
    }
    
    // Initialize Terminator cursor effect
    initTerminatorCursor();
    
    // Initialize Terminator navigation effects
    initTerminatorNav();
});

/**
 * Initialize dark mode toggle functionality
 */
function initDarkMode() {
    const darkModeToggle = document.getElementById('darkModeToggle');
    const body = document.body;
    
    // Check for saved user preference
    if (localStorage.getItem('darkMode') === 'enabled') {
        darkModeToggle.checked = true;
        body.classList.add('dark-mode');
    }
    
    // Toggle dark mode
    darkModeToggle.addEventListener('change', function() {
        if (this.checked) {
            body.classList.add('dark-mode');
            localStorage.setItem('darkMode', 'enabled');
        } else {
            body.classList.remove('dark-mode');
            localStorage.setItem('darkMode', 'disabled');
        }
    });
}

/**
 * Initialize terminal copy functionality
 */
function initTerminalCopy() {
    const copyButton = document.querySelector('.copy-btn');
    
    copyButton.addEventListener('click', function() {
        const cmdElement = document.querySelector('.terminal-command');
        const cmdText = cmdElement ? cmdElement.getAttribute('data-cmd') : '';
        
        if (!cmdText) return;
        
        // Try to use Clipboard API first
        navigator.clipboard.writeText(`npx ${cmdText}`)
            .then(() => {
                showCopySuccess(this);
            })
            .catch(() => {
                // Fallback for browsers without Clipboard API
                useFallbackCopyMethod(cmdText);
            });
    });
}

/**
 * Initialize Terminator-style cursor effect
 */
function initTerminatorCursor() {
    // Don't initialize on mobile devices
    if (window.innerWidth < 768) return;
    
    const elements = {
        cursor: document.createElement('div'),
        target: document.createElement('div'),
        scanLine: document.createElement('div'),
        crosshair: document.createElement('div'),
        hud: document.createElement('div'),
        particles: []
    };
    
    // Create cursor elements
    createTerminatorElements(elements);
    
    // Set up event listeners
    setupTerminatorListeners(elements);
    
    // Start animation loop
    animateTerminatorCursor(elements);
}

function createTerminatorElements(elements) {
    // Main targeting cursor (red dot)
    elements.cursor.className = 'terminator-cursor';
    document.body.appendChild(elements.cursor);
    
    // Target acquisition circle
    elements.target.className = 'terminator-target';
    document.body.appendChild(elements.target);
    
    // Scanning line (horizontal)
    elements.scanLine.className = 'terminator-scan-line';
    document.body.appendChild(elements.scanLine);
    
    // Crosshair lines
    elements.crosshair.className = 'terminator-crosshair';
    document.body.appendChild(elements.crosshair);
    
    // HUD display
    elements.hud.className = 'terminator-hud';
    elements.hud.innerHTML = `
        <div class="hud-coords">X: 0 Y: 0</div>
        <div class="hud-target">TARGET: NONE</div>
        <div class="hud-status">SYSTEMS: ONLINE</div>
    `;
    document.body.appendChild(elements.hud);
    
    // Hide default cursor
    document.body.style.cursor = 'none';
}

function setupTerminatorListeners(elements) {
    let mouseX = 0;
    let mouseY = 0;
    let targetX = 0;
    let targetY = 0;
    let scanProgress = 0;
    let currentTarget = null;
    
    document.addEventListener('mousemove', (e) => {
        mouseX = e.clientX;
        mouseY = e.clientY;
        
        // Update HUD coordinates
        elements.hud.querySelector('.hud-coords').textContent = 
            `X: ${mouseX} Y: ${mouseY}`;
        
        // Check for elements under cursor
        const hoveredElement = document.elementFromPoint(e.clientX, e.clientY);
        if (hoveredElement !== currentTarget) {
            currentTarget = hoveredElement;
            elements.hud.querySelector('.hud-target').textContent = 
                `TARGET: ${currentTarget?.tagName || 'NONE'}`;
        }
        
        // Create scanning particles
        if (Math.random() > 0.8) {
            createTerminatorParticle(elements, mouseX, mouseY);
        }
    });
    
    // Store references for animation loop
    elements.getMousePos = () => ({ x: mouseX, y: mouseY });
    elements.getTargetPos = () => ({ x: targetX, y: targetY });
    elements.setTargetPos = (x, y) => {
        targetX = x;
        targetY = y;
        elements.target.style.left = `${x}px`;
        elements.target.style.top = `${y}px`;
    };
    elements.scanPosition = () => scanProgress;
    elements.advanceScan = () => {
        scanProgress = (scanProgress + 0.02) % 1;
        return scanProgress;
    };
}

function createTerminatorParticle(elements, x, y) {
    const particle = document.createElement('div');
    particle.className = 'terminator-particle';
    
    const size = Math.random() * 3 + 1;
    const angle = Math.random() * Math.PI * 2;
    const velocity = Math.random() * 2 + 1;
    
    particle.style.cssText = `
        position: fixed;
        width: ${size}px;
        height: ${size}px;
        background-color: rgba(255, 50, 50, 0.7);
        border-radius: 50%;
        pointer-events: none;
        z-index: 9997;
        left: ${x}px;
        top: ${y}px;
        transform: translate(-50%, -50%);
        box-shadow: 0 0 5px 1px rgba(255, 0, 0, 0.8);
    `;
    
    document.body.appendChild(particle);
    elements.particles.push({
        element: particle,
        x: x,
        y: y,
        vx: Math.cos(angle) * velocity,
        vy: Math.sin(angle) * velocity,
        life: 50 + Math.random() * 50
    });
}

function animateTerminatorCursor(elements) {
    // Update main cursor position
    const mouse = elements.getMousePos();
    elements.cursor.style.left = `${mouse.x}px`;
    elements.cursor.style.top = `${mouse.y}px`;
    
    // Update target circle with delay
    const target = elements.getTargetPos();
    const dx = mouse.x - target.x;
    const dy = mouse.y - target.y;
    elements.setTargetPos(
        target.x + dx * 0.1,
        target.y + dy * 0.1
    );
    
    // Update scanning line
    const scanPos = elements.advanceScan();
    elements.scanLine.style.left = `${mouse.x}px`;
    elements.scanLine.style.top = `${mouse.y + (scanPos - 0.5) * 40}px`;
    
    // Update crosshair
    elements.crosshair.style.left = `${mouse.x}px`;
    elements.crosshair.style.top = `${mouse.y}px`;
    
    // Update HUD position (offset from cursor)
    elements.hud.style.left = `${mouse.x + 20}px`;
    elements.hud.style.top = `${mouse.y + 20}px`;
    
    // Update particles
    elements.particles.forEach((p, i) => {
        p.x += p.vx;
        p.y += p.vy;
        p.life--;
        
        p.element.style.left = `${p.x}px`;
        p.element.style.top = `${p.y}px`;
        p.element.style.opacity = p.life / 100;
        
        if (p.life <= 0) {
            p.element.remove();
            elements.particles.splice(i, 1);
        }
    });
    
    requestAnimationFrame(() => animateTerminatorCursor(elements));
}

/**
 * Initialize Terminator navigation effects
 */
function initTerminatorNav() {
    const navLinks = document.querySelectorAll('.terminator-link');
    
    navLinks.forEach(link => {
        // Add hover effects
        link.addEventListener('mouseenter', function() {
            this.querySelector('.terminator-link-status').textContent = '[ACTIVE]';
            
            // Create a subtle glitch effect
            if (Math.random() > 0.7) {
                this.style.textShadow = '0 0 8px rgba(255, 0, 0, 0.8)';
                setTimeout(() => {
                    this.style.textShadow = 'none';
                }, 100);
            }
        });
        
        link.addEventListener('mouseleave', function() {
            if (!this.classList.contains('active')) {
                this.querySelector('.terminator-link-status').textContent = '[STANDBY]';
            }
        });
        
        // Add click effect
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Remove active class from all links
            navLinks.forEach(l => {
                l.classList.remove('active');
                l.querySelector('.terminator-link-status').textContent = '[STANDBY]';
            });
            
            // Add active class to clicked link
            this.classList.add('active');
            this.querySelector('.terminator-link-status').textContent = '[ACTIVE]';
            
            // Add visual feedback
            this.style.transform = 'scale(0.95)';
            setTimeout(() => {
                this.style.transform = '';
            }, 200);
            
            // Simulate system processing
            const originalText = this.textContent;
            this.textContent = '> PROCESSING...';
            setTimeout(() => {
                this.innerHTML = originalText;
            }, 500);
        });
    });
    
    // Add random system status updates
    setInterval(() => {
        const status = document.querySelector('.terminator-status');
        if (status && Math.random() > 0.8) {
            const statuses = ['[ONLINE]', '[SYNCING]', '[SCANNING]', '[ACTIVE]'];
            const originalText = status.textContent;
            status.textContent = statuses[Math.floor(Math.random() * statuses.length)];
            
            setTimeout(() => {
                status.textContent = originalText;
            }, 1000);
        }
    }, 5000);
}

/**
 * Show visual feedback when copy is successful
 * @param {HTMLElement} button - The copy button element
 */
function showCopySuccess(button) {
    const svg = button.querySelector('svg');
    if (!svg) return;
    
    // Show checkmark icon
    svg.innerHTML = `
        <path d="M9 5h-2a2 2 0 0 0 -2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2 -2v-12a2 2 0 0 0 -2 -2h-2"></path>
        <path d="M9 3m0 2a2 2 0 0 1 2 -2h2a2 2 0 0 1 2 2v0a2 2 0 0 1 -2 2h-2a2 2 0 0 1 -2 -2z"></path>
        <path d="M9 13l2 2l4 -4"></path>
    `;
    
    // Revert to copy icon after 1 second
    setTimeout(() => {
        svg.innerHTML = `
            <path d="M9 5h-2a2 2 0 0 0 -2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2 -2v-12a2 2 0 0 0 -2 -2h-2"></path>
            <path d="M9 3m0 2a2 2 0 0 1 2 -2h2a2 2 0 0 1 2 2v0a2 2 0 0 1 -2 2h-2a2 2 0 0 1 -2 -2z"></path>
        `;
    }, 1000);
}

/**
 * Fallback copy method for browsers without Clipboard API
 * @param {string} text - The text to copy
 */
function useFallbackCopyMethod(text) {
    const textArea = document.createElement('textarea');
    textArea.value = `npx ${text}`;
    document.body.appendChild(textArea);
    textArea.select();
    
    try {
        document.execCommand('copy');
    } catch (err) {
        console.error('Fallback copy failed: ', err);
    }
    
    document.body.removeChild(textArea);
}