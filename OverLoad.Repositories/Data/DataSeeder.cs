using Microsoft.EntityFrameworkCore;
using OverLoad.Domain.Entities;
using OverLoad.Domain.Enums;

namespace OverLoad.Repositories.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Seed / Reset passwords for Admin and Instructor accounts
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@overload.io");
        if (adminUser != null)
        {
            adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456");
            adminUser.Role = UserRole.Admin;
        }
        else
        {
            context.Users.Add(new User
            {
                Email = "admin@overload.io",
                FullName = "System Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456"),
                Role = UserRole.Admin,
                IsVerified = true
            });
        }

        var instructorUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "john.instructor@overload.io");
        if (instructorUser != null)
        {
            instructorUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Instructor@123456");
            instructorUser.Role = UserRole.Instructor;
        }
        else
        {
            context.Users.Add(new User
            {
                Email = "john.instructor@overload.io",
                FullName = "John Carter",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Instructor@123456"),
                Role = UserRole.Instructor,
                IsVerified = true
            });
        }

        var existingSlugs = await context.Courses.Select(c => c.Slug).ToListAsync();
        var courses = new List<Course>();

        void AddIfMissing(Course c)
        {
            if (!existingSlugs.Contains(c.Slug))
                courses.Add(c);
        }

        // ═══════════════════════════════════════════════════════════════════
        // FRONTEND COURSES
        // ═══════════════════════════════════════════════════════════════════

        // 1. HTML & CSS
        AddIfMissing(CreateCourse("HTML & CSS Cơ Bản cho Người Mới", "html-css-co-ban", "Frontend", CourseLevel.Beginner, 0,
            "Học cách xây dựng giao diện web từ đầu với HTML5 và CSS3. Phù hợp cho người mới bắt đầu lập trình web.",
            new[]
            {
                ("Introduction to HTML", "Tổng quan về HTML và cách trình duyệt render trang web.", 12, true,
@"# Introduction to HTML

HTML (HyperText Markup Language) là ngôn ngữ đánh dấu tiêu chuẩn để tạo trang web. Mọi trang web bạn thấy trên Internet đều được xây dựng bằng HTML.

## Cấu trúc cơ bản

```html
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Trang đầu tiên</title>
</head>
<body>
    <h1>Xin chào thế giới!</h1>
    <p>Đây là trang web đầu tiên của tôi.</p>
</body>
</html>
```

## Các thẻ HTML quan trọng

- `<h1>` đến `<h6>`: Tiêu đề với các mức độ khác nhau
- `<p>`: Đoạn văn bản
- `<a>`: Liên kết
- `<img>`: Hình ảnh
- `<div>`: Phần tử khối chứa nội dung
- `<span>`: Phần tử inline

Hãy thực hành tạo một trang HTML đơn giản và mở trong trình duyệt để xem kết quả."),

                ("HTML Elements & Attributes", "Tìm hiểu các phần tử HTML phổ biến và thuộc tính của chúng.", 15, false,
@"# HTML Elements & Attributes

## Forms và Input

Forms là cách chính để thu thập dữ liệu từ người dùng:

```html
<form action=""/submit"" method=""POST"">
    <label for=""email"">Email:</label>
    <input type=""email"" id=""email"" name=""email"" required>
    
    <label for=""password"">Mật khẩu:</label>
    <input type=""password"" id=""password"" name=""password"" minlength=""6"">
    
    <button type=""submit"">Đăng nhập</button>
</form>
```

## Semantic HTML5

HTML5 cung cấp các thẻ semantic giúp cấu trúc trang rõ ràng hơn:

```html
<header>Phần đầu trang</header>
<nav>Thanh điều hướng</nav>
<main>
    <article>Nội dung bài viết</article>
    <aside>Thanh bên</aside>
</main>
<footer>Phần cuối trang</footer>
```

Sử dụng semantic HTML giúp SEO tốt hơn và accessibility cho người dùng screen reader."),

                ("CSS Selectors & Properties", "Học cách chọn phần tử và áp dụng style với CSS.", 18, false,
@"# CSS Selectors & Properties

## Các loại Selector

```css
/* Element selector */
p { color: #333; }

/* Class selector */
.highlight { background-color: yellow; }

/* ID selector */
#main-title { font-size: 2rem; }

/* Descendant selector */
.card p { margin-bottom: 1rem; }

/* Pseudo-class */
a:hover { color: #0066cc; text-decoration: underline; }

/* Attribute selector */
input[type=""email""] { border: 2px solid #4CAF50; }
```

## Box Model

Mỗi phần tử HTML được bao bọc bởi Box Model gồm: content, padding, border, margin.

```css
.box {
    width: 300px;
    padding: 20px;
    border: 1px solid #ddd;
    margin: 10px auto;
    box-sizing: border-box; /* Bao gồm padding và border trong width */
}
```"),

                ("Flexbox Layout", "Xây dựng layout linh hoạt với CSS Flexbox.", 20, false,
@"# Flexbox Layout

Flexbox là phương pháp layout một chiều mạnh mẽ trong CSS.

## Cách sử dụng cơ bản

```css
.container {
    display: flex;
    justify-content: space-between; /* Căn ngang */
    align-items: center;            /* Căn dọc */
    gap: 1rem;                      /* Khoảng cách giữa items */
}

.item {
    flex: 1;           /* Chiếm đều không gian */
    padding: 1rem;
}

/* Responsive: chuyển sang cột trên mobile */
@media (max-width: 768px) {
    .container {
        flex-direction: column;
    }
}
```

## Ví dụ thực tế: Navigation Bar

```css
.navbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem 2rem;
    background: #1a1a2e;
    color: white;
}

.nav-links {
    display: flex;
    gap: 2rem;
    list-style: none;
}
```"),

                ("CSS Grid", "Tạo layout 2 chiều phức tạp với CSS Grid.", 22, false,
@"# CSS Grid Layout

CSS Grid cho phép bạn tạo layout 2 chiều (hàng và cột) một cách dễ dàng.

```css
.grid-container {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    grid-template-rows: auto;
    gap: 1.5rem;
    padding: 2rem;
}

/* Card chiếm 2 cột */
.featured-card {
    grid-column: span 2;
}

/* Responsive grid */
@media (max-width: 768px) {
    .grid-container {
        grid-template-columns: 1fr;
    }
    .featured-card {
        grid-column: span 1;
    }
}
```

## Grid Areas

```css
.layout {
    display: grid;
    grid-template-areas:
        'header header header'
        'sidebar main main'
        'footer footer footer';
    grid-template-columns: 250px 1fr 1fr;
    min-height: 100vh;
}

.header  { grid-area: header; }
.sidebar { grid-area: sidebar; }
.main    { grid-area: main; }
.footer  { grid-area: footer; }
```"),

                ("Responsive Design & Media Queries", "Thiết kế web thích ứng cho mọi thiết bị.", 20, false,
@"# Responsive Design

## Mobile-First Approach

Thiết kế cho mobile trước, sau đó mở rộng cho màn hình lớn:

```css
/* Base: Mobile */
.container {
    padding: 1rem;
    font-size: 14px;
}

/* Tablet */
@media (min-width: 768px) {
    .container {
        padding: 2rem;
        font-size: 16px;
        max-width: 720px;
        margin: 0 auto;
    }
}

/* Desktop */
@media (min-width: 1024px) {
    .container {
        max-width: 1200px;
    }
}
```

## Responsive Images

```css
img {
    max-width: 100%;
    height: auto;
    object-fit: cover;
}

/* Art direction với picture element */
```

```html
<picture>
    <source media=""(min-width: 1024px)"" srcset=""hero-desktop.jpg"">
    <source media=""(min-width: 768px)"" srcset=""hero-tablet.jpg"">
    <img src=""hero-mobile.jpg"" alt=""Hero banner"">
</picture>
```

Luôn test trên nhiều kích thước màn hình khác nhau trước khi deploy.")
            }));

        // 2. JavaScript ES6+
        AddIfMissing(CreateCourse("JavaScript ES6+ Toàn Diện", "javascript-es6-toan-dien", "Frontend", CourseLevel.Beginner, 99000,
            "Nắm vững JavaScript hiện đại từ cú pháp ES6+ đến async/await. Nền tảng vững chắc cho React, Node.js.",
            new[]
            {
                ("Variables, Data Types & Operators", "Biến, kiểu dữ liệu và toán tử trong JavaScript.", 15, true,
@"# Variables & Data Types

## let, const vs var

```javascript
// const: không thể gán lại (khuyên dùng mặc định)
const API_URL = 'https://api.example.com';

// let: có thể gán lại, block-scoped
let count = 0;
count = 1; // OK

// var: function-scoped, tránh sử dụng
var oldWay = 'legacy';
```

## Kiểu dữ liệu

```javascript
// Primitive types
const name = 'Nguyễn Văn A';   // string
const age = 25;                  // number
const isStudent = true;          // boolean
const empty = null;              // null
const notDefined = undefined;    // undefined
const uniqueId = Symbol('id');   // symbol
const bigNum = 9007199254740991n; // bigint

// Reference types
const user = { name, age };      // object
const scores = [85, 92, 78];     // array
```"),

                ("Functions & Arrow Functions", "Hàm truyền thống và arrow functions trong ES6+.", 18, false,
@"# Functions in JavaScript

## Arrow Functions

```javascript
// Traditional function
function add(a, b) {
    return a + b;
}

// Arrow function
const add = (a, b) => a + b;

// Arrow function với body
const greet = (name) => {
    const message = `Xin chào, ${name}!`;
    console.log(message);
    return message;
};

// Default parameters
const createUser = (name, role = 'Student') => ({
    name,
    role,
    createdAt: new Date()
});
```

## Higher-Order Functions

```javascript
const numbers = [1, 2, 3, 4, 5];

const doubled = numbers.map(n => n * 2);       // [2, 4, 6, 8, 10]
const evens = numbers.filter(n => n % 2 === 0); // [2, 4]
const sum = numbers.reduce((acc, n) => acc + n, 0); // 15
```"),

                ("Destructuring & Spread Operator", "Phân rã đối tượng/mảng và toán tử spread/rest.", 15, false,
@"# Destructuring & Spread

## Object Destructuring

```javascript
const user = { name: 'An', email: 'an@mail.com', age: 22 };

// Destructure
const { name, email, age = 18 } = user;

// Rename
const { name: userName, email: userEmail } = user;

// Nested
const course = { title: 'React', info: { level: 'Intermediate', price: 199000 } };
const { info: { level, price } } = course;
```

## Spread Operator

```javascript
// Copy array
const original = [1, 2, 3];
const copy = [...original, 4, 5]; // [1, 2, 3, 4, 5]

// Merge objects
const defaults = { theme: 'dark', lang: 'vi' };
const userSettings = { lang: 'en', fontSize: 14 };
const merged = { ...defaults, ...userSettings };
// { theme: 'dark', lang: 'en', fontSize: 14 }
```"),

                ("Template Literals & String Methods", "Chuỗi template và các phương thức xử lý chuỗi.", 12, false,
@"# Template Literals

## Tagged Templates

```javascript
const name = 'Minh';
const course = 'JavaScript';

// Template literal
const message = `Xin chào ${name}, bạn đang học ${course}!`;

// Multi-line
const html = `
    <div class=""card"">
        <h2>${course}</h2>
        <p>Học viên: ${name}</p>
    </div>
`;
```

## Useful String Methods

```javascript
const str = '  Hello World  ';
str.trim();           // 'Hello World'
str.includes('World'); // true
str.startsWith('  He'); // true
'abc'.repeat(3);       // 'abcabcabc'
'hello'.padStart(10, '*'); // '*****hello'
```"),

                ("Arrays & Modern Array Methods", "Các phương thức mảng hiện đại: map, filter, reduce, find.", 20, false,
@"# Modern Array Methods

```javascript
const students = [
    { name: 'An', score: 85, passed: true },
    { name: 'Bình', score: 62, passed: true },
    { name: 'Chi', score: 45, passed: false },
    { name: 'Dũng', score: 92, passed: true }
];

// find - tìm phần tử đầu tiên thỏa điều kiện
const topStudent = students.find(s => s.score > 90);
// { name: 'Dũng', score: 92, passed: true }

// filter - lọc các phần tử thỏa điều kiện
const passedStudents = students.filter(s => s.passed);

// map - biến đổi từng phần tử
const names = students.map(s => s.name);
// ['An', 'Bình', 'Chi', 'Dũng']

// reduce - tổng hợp giá trị
const avgScore = students.reduce((sum, s) => sum + s.score, 0) / students.length;

// sort
const sorted = [...students].sort((a, b) => b.score - a.score);

// some & every
const allPassed = students.every(s => s.passed); // false
const anyFailed = students.some(s => !s.passed); // true
```"),

                ("Promises & Async/Await", "Xử lý bất đồng bộ với Promise và async/await.", 25, false,
@"# Async JavaScript

## Promises

```javascript
const fetchUser = (id) => {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            if (id > 0) resolve({ id, name: 'User ' + id });
            else reject(new Error('Invalid ID'));
        }, 1000);
    });
};

fetchUser(1)
    .then(user => console.log(user))
    .catch(err => console.error(err));
```

## Async/Await

```javascript
const loadDashboard = async () => {
    try {
        const user = await fetchUser(1);
        const courses = await fetch(`/api/users/${user.id}/courses`);
        const data = await courses.json();
        
        console.log('Dashboard loaded:', data);
        return data;
    } catch (error) {
        console.error('Failed to load dashboard:', error.message);
    }
};

// Parallel requests
const loadAll = async () => {
    const [users, courses, stats] = await Promise.all([
        fetch('/api/users').then(r => r.json()),
        fetch('/api/courses').then(r => r.json()),
        fetch('/api/stats').then(r => r.json())
    ]);
};
```"),

                ("Modules (import/export)", "Hệ thống module ES6: import, export, dynamic import.", 15, false,
@"# ES6 Modules

## Named Exports

```javascript
// utils.js
export const formatPrice = (price) => 
    price.toLocaleString('vi-VN') + 'đ';

export const slugify = (str) =>
    str.toLowerCase().replace(/\s+/g, '-');

// app.js
import { formatPrice, slugify } from './utils.js';
console.log(formatPrice(199000)); // '199.000đ'
```

## Default Export

```javascript
// UserService.js
export default class UserService {
    async getAll() { /* ... */ }
    async getById(id) { /* ... */ }
}

// app.js
import UserService from './UserService.js';
const service = new UserService();
```

## Dynamic Import

```javascript
// Lazy loading - chỉ load khi cần
const loadChart = async () => {
    const { Chart } = await import('./chart.js');
    return new Chart('#canvas');
};
```"),

                ("Error Handling & Debugging", "Xử lý lỗi với try/catch và kỹ thuật debug.", 14, false,
@"# Error Handling

## try/catch/finally

```javascript
const parseJSON = (str) => {
    try {
        const data = JSON.parse(str);
        return { success: true, data };
    } catch (error) {
        console.error('Parse failed:', error.message);
        return { success: false, error: error.message };
    } finally {
        console.log('Parse attempt completed');
    }
};
```

## Custom Errors

```javascript
class ValidationError extends Error {
    constructor(field, message) {
        super(message);
        this.name = 'ValidationError';
        this.field = field;
    }
}

const validateEmail = (email) => {
    if (!email.includes('@')) {
        throw new ValidationError('email', 'Email không hợp lệ');
    }
    return true;
};
```")
            }));

        // 3. React.js
        AddIfMissing(CreateCourse("React.js Từ Zero Đến Hero", "reactjs-zero-hero", "Frontend", CourseLevel.Intermediate, 199000,
            "Xây dựng ứng dụng web hiện đại với React 19. Từ component cơ bản đến hooks nâng cao và state management.",
            new[]
            {
                ("React Components & JSX", "Hiểu về component, JSX và cách render UI trong React.", 15, true,
@"# React Components & JSX

## Functional Component

```jsx
function CourseCard({ title, price, level }) {
    return (
        <div className=""course-card"">
            <h3>{title}</h3>
            <span className=""badge"">{level}</span>
            <p className=""price"">
                {price === 0 ? 'Miễn phí' : `${price.toLocaleString('vi-VN')}đ`}
            </p>
        </div>
    );
}

// Sử dụng
<CourseCard title=""React Basics"" price={199000} level=""Intermediate"" />
```

## Conditional Rendering

```jsx
function UserStatus({ isLoggedIn, name }) {
    return (
        <div>
            {isLoggedIn ? (
                <p>Chào mừng, {name}!</p>
            ) : (
                <button>Đăng nhập</button>
            )}
        </div>
    );
}
```"),

                ("useState & useEffect Hooks", "Quản lý state và side effects với React Hooks.", 20, false,
@"# React Hooks

## useState

```jsx
import { useState } from 'react';

function Counter() {
    const [count, setCount] = useState(0);
    
    return (
        <div>
            <p>Đếm: {count}</p>
            <button onClick={() => setCount(prev => prev + 1)}>+1</button>
            <button onClick={() => setCount(0)}>Reset</button>
        </div>
    );
}
```

## useEffect

```jsx
import { useState, useEffect } from 'react';

function CourseList() {
    const [courses, setCourses] = useState([]);
    const [loading, setLoading] = useState(true);
    
    useEffect(() => {
        fetch('/api/courses')
            .then(res => res.json())
            .then(data => {
                setCourses(data.items);
                setLoading(false);
            });
    }, []); // [] = chạy 1 lần khi mount
    
    if (loading) return <p>Đang tải...</p>;
    
    return (
        <ul>
            {courses.map(c => <li key={c.id}>{c.title}</li>)}
        </ul>
    );
}
```"),

                ("Event Handling & Forms", "Xử lý sự kiện và quản lý form trong React.", 18, false,
@"# Event Handling & Forms

## Controlled Components

```jsx
function LoginForm() {
    const [form, setForm] = useState({ email: '', password: '' });
    const [errors, setErrors] = useState({});
    
    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm(prev => ({ ...prev, [name]: value }));
    };
    
    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!form.email.includes('@')) {
            setErrors({ email: 'Email không hợp lệ' });
            return;
        }
        const res = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(form)
        });
        const data = await res.json();
        if (data.success) window.location.href = '/dashboard';
    };
    
    return (
        <form onSubmit={handleSubmit}>
            <input name=""email"" value={form.email} onChange={handleChange} />
            {errors.email && <span className=""error"">{errors.email}</span>}
            <input name=""password"" type=""password"" value={form.password} onChange={handleChange} />
            <button type=""submit"">Đăng nhập</button>
        </form>
    );
}
```"),

                ("Lists, Keys & Conditional Rendering", "Render danh sách, sử dụng key và conditional rendering.", 16, false,
@"# Lists & Conditional Rendering

## Rendering Lists

```jsx
function StudentList({ students }) {
    if (students.length === 0) {
        return <p className=""empty"">Chưa có học viên nào.</p>;
    }
    
    return (
        <div className=""student-grid"">
            {students.map(student => (
                <div key={student.id} className=""student-card"">
                    <img src={student.avatar} alt={student.name} />
                    <h4>{student.name}</h4>
                    <span className={`status ${student.isActive ? 'active' : 'inactive'}`}>
                        {student.isActive ? 'Đang học' : 'Tạm nghỉ'}
                    </span>
                </div>
            ))}
        </div>
    );
}
```

Luôn sử dụng `key` duy nhất khi render list. Tránh dùng index làm key nếu list có thể thay đổi thứ tự."),

                ("useContext & Global State", "Chia sẻ state toàn cục với Context API.", 22, false,
@"# Context API

## Tạo Auth Context

```jsx
import { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    
    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            fetch('/api/auth/me', {
                headers: { Authorization: `Bearer ${token}` }
            })
            .then(r => r.json())
            .then(data => setUser(data.user))
            .finally(() => setLoading(false));
        } else {
            setLoading(false);
        }
    }, []);
    
    const login = async (email, password) => { /* ... */ };
    const logout = () => { localStorage.removeItem('token'); setUser(null); };
    
    return (
        <AuthContext.Provider value={{ user, login, logout, loading }}>
            {children}
        </AuthContext.Provider>
    );
}

export const useAuth = () => useContext(AuthContext);
```"),

                ("Custom Hooks", "Tạo custom hooks để tái sử dụng logic.", 18, false,
@"# Custom Hooks

## useFetch Hook

```jsx
function useFetch(url) {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    
    useEffect(() => {
        const controller = new AbortController();
        
        setLoading(true);
        fetch(url, { signal: controller.signal })
            .then(r => r.json())
            .then(setData)
            .catch(err => {
                if (err.name !== 'AbortError') setError(err);
            })
            .finally(() => setLoading(false));
        
        return () => controller.abort(); // Cleanup
    }, [url]);
    
    return { data, loading, error };
}

// Sử dụng
function CoursePage({ id }) {
    const { data: course, loading, error } = useFetch(`/api/courses/${id}`);
    
    if (loading) return <Spinner />;
    if (error) return <ErrorMessage message={error.message} />;
    return <CourseDetail course={course} />;
}
```"),

                ("React Router & Navigation", "Điều hướng SPA với React Router v6.", 20, false,
@"# React Router v6

## Cấu hình Routes

```jsx
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path=""/"" element={<HomePage />} />
                <Route path=""/courses"" element={<CoursesPage />} />
                <Route path=""/course/:slug"" element={<CourseDetail />} />
                <Route path=""/admin"" element={
                    <ProtectedRoute role=""Admin"">
                        <AdminDashboard />
                    </ProtectedRoute>
                } />
                <Route path=""*"" element={<Navigate to=""/"" />} />
            </Routes>
        </BrowserRouter>
    );
}
```

## Protected Route

```jsx
function ProtectedRoute({ children, role }) {
    const { user, loading } = useAuth();
    if (loading) return <Spinner />;
    if (!user) return <Navigate to=""/login"" />;
    if (role && user.role !== role) return <Navigate to=""/"" />;
    return children;
}
```"),

                ("Performance Optimization", "Tối ưu hiệu suất với memo, useMemo, useCallback.", 20, false,
@"# React Performance

## React.memo

```jsx
const CourseCard = React.memo(function CourseCard({ title, price }) {
    console.log('Render:', title);
    return (
        <div className=""card"">
            <h3>{title}</h3>
            <p>{price.toLocaleString('vi-VN')}đ</p>
        </div>
    );
});
```

## useMemo & useCallback

```jsx
function CourseList({ courses, filter }) {
    // Chỉ tính lại khi courses hoặc filter thay đổi
    const filteredCourses = useMemo(() => {
        return courses.filter(c => 
            c.category === filter || filter === 'all'
        );
    }, [courses, filter]);
    
    // Giữ reference ổn định cho callback
    const handleEnroll = useCallback((courseId) => {
        fetch(`/api/enrollments`, {
            method: 'POST',
            body: JSON.stringify({ courseId })
        });
    }, []);
    
    return filteredCourses.map(c => (
        <CourseCard key={c.id} {...c} onEnroll={handleEnroll} />
    ));
}
```")
            }));

        // 4. Next.js 15
        AddIfMissing(CreateCourse("Next.js 15 & App Router", "nextjs-15-app-router", "Frontend", CourseLevel.Advanced, 299000,
            "Phát triển ứng dụng full-stack với Next.js 15 App Router, Server Components và Server Actions.",
            new[]
            {
                ("Next.js Project Structure & App Router", "Cấu trúc dự án Next.js 15 với App Router.", 18, true,
@"# Next.js 15 App Router

## Cấu trúc thư mục

```
app/
├── layout.tsx          # Root layout
├── page.tsx            # Home page (/)
├── globals.css
├── courses/
│   ├── page.tsx        # /courses
│   └── [slug]/
│       └── page.tsx    # /courses/:slug
├── admin/
│   ├── layout.tsx      # Admin layout
│   └── page.tsx        # /admin
└── api/
    └── courses/
        └── route.ts    # API route
```

## Root Layout

```tsx
// app/layout.tsx
export default function RootLayout({ children }: { children: React.ReactNode }) {
    return (
        <html lang=""vi"">
            <body>
                <Navbar />
                <main>{children}</main>
                <Footer />
            </body>
        </html>
    );
}
```"),

                ("Server Components vs Client Components", "Phân biệt và sử dụng đúng Server/Client Components.", 22, false,
@"# Server vs Client Components

## Server Component (default)

```tsx
// app/courses/page.tsx - Server Component
async function CoursesPage() {
    const res = await fetch('https://api.example.com/courses', {
        cache: 'no-store' // or next: { revalidate: 3600 }
    });
    const courses = await res.json();
    
    return (
        <div>
            <h1>Khóa học</h1>
            {courses.map(c => <CourseCard key={c.id} course={c} />)}
        </div>
    );
}
```

## Client Component

```tsx
'use client'; // Phải khai báo ở đầu file

import { useState } from 'react';

export function SearchBar({ onSearch }: { onSearch: (q: string) => void }) {
    const [query, setQuery] = useState('');
    
    return (
        <input
            value={query}
            onChange={(e) => {
                setQuery(e.target.value);
                onSearch(e.target.value);
            }}
            placeholder=""Tìm khóa học...""
        />
    );
}
```"),

                ("Data Fetching & Caching", "Fetch dữ liệu với cache strategies trong Next.js.", 20, false,
@"# Data Fetching in Next.js 15

## Static vs Dynamic Rendering

```tsx
// Static (cached indefinitely)
const data = await fetch('https://api.example.com/courses', {
    cache: 'force-cache'
});

// Revalidate every hour
const data = await fetch('https://api.example.com/courses', {
    next: { revalidate: 3600 }
});

// Dynamic (no cache)
const data = await fetch('https://api.example.com/courses', {
    cache: 'no-store'
});
```

## Parallel Data Fetching

```tsx
async function DashboardPage() {
    // Fetch đồng thời - KHÔNG chờ tuần tự
    const [coursesRes, statsRes, usersRes] = await Promise.all([
        fetch('/api/courses'),
        fetch('/api/stats'),
        fetch('/api/users')
    ]);
    
    const courses = await coursesRes.json();
    const stats = await statsRes.json();
    const users = await usersRes.json();
    
    return <Dashboard courses={courses} stats={stats} users={users} />;
}
```"),

                ("Server Actions & Form Handling", "Xử lý form submissions với Server Actions.", 25, false,
@"# Server Actions

## Inline Server Action

```tsx
// app/courses/new/page.tsx
export default function NewCoursePage() {
    async function createCourse(formData: FormData) {
        'use server';
        
        const title = formData.get('title') as string;
        const category = formData.get('category') as string;
        const price = Number(formData.get('price'));
        
        await fetch('https://api.example.com/courses', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ title, category, price })
        });
        
        redirect('/courses');
    }
    
    return (
        <form action={createCourse}>
            <input name=""title"" required />
            <select name=""category"">
                <option value=""Frontend"">Frontend</option>
                <option value=""Backend"">Backend</option>
                <option value=""Database"">Database</option>
            </select>
            <input name=""price"" type=""number"" />
            <button type=""submit"">Tạo khóa học</button>
        </form>
    );
}
```"),

                ("Dynamic Routes & Metadata", "Routes động và SEO metadata trong Next.js.", 18, false,
@"# Dynamic Routes & Metadata

## Dynamic Route

```tsx
// app/course/[slug]/page.tsx
interface Props {
    params: { slug: string };
}

export async function generateMetadata({ params }: Props) {
    const course = await getCourse(params.slug);
    return {
        title: `${course.title} | ScrollTutor`,
        description: course.description,
        openGraph: {
            title: course.title,
            images: [course.thumbnailUrl]
        }
    };
}

export default async function CoursePage({ params }: Props) {
    const course = await getCourse(params.slug);
    
    return (
        <article>
            <h1>{course.title}</h1>
            <p>{course.description}</p>
            <LessonList lessons={course.lessons} />
        </article>
    );
}
```"),

                ("Middleware & Authentication", "Bảo vệ routes với Next.js Middleware.", 20, false,
@"# Next.js Middleware

## Authentication Middleware

```tsx
// middleware.ts (root of project)
import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
    const token = request.cookies.get('token')?.value;
    const { pathname } = request.nextUrl;
    
    // Protect admin routes
    if (pathname.startsWith('/admin')) {
        if (!token) {
            return NextResponse.redirect(new URL('/login', request.url));
        }
    }
    
    // Redirect logged-in users away from login
    if (pathname === '/login' && token) {
        return NextResponse.redirect(new URL('/', request.url));
    }
    
    return NextResponse.next();
}

export const config = {
    matcher: ['/admin/:path*', '/login']
};
```"),

                ("Deployment & Production Optimization", "Deploy ứng dụng Next.js lên production.", 15, false,
@"# Deployment & Optimization

## Build cho Production

```bash
# Build
npm run build

# Kiểm tra bundle size
npx @next/bundle-analyzer
```

## Environment Variables

```env
# .env.local (không push lên git)
DATABASE_URL=postgresql://user:pass@localhost:5432/mydb
NEXT_PUBLIC_API_URL=https://api.scrolltutor.com
JWT_SECRET=my-secret-key
```

## Image Optimization

```tsx
import Image from 'next/image';

function CourseThumb({ src, title }) {
    return (
        <Image
            src={src}
            alt={title}
            width={400}
            height={225}
            placeholder=""blur""
            blurDataURL=""data:image/jpeg;base64,..."" 
            priority={false}
        />
    );
}
```

## Caching Headers

```tsx
// next.config.js
module.exports = {
    async headers() {
        return [{
            source: '/api/:path*',
            headers: [
                { key: 'Cache-Control', value: 's-maxage=60, stale-while-revalidate=300' }
            ]
        }];
    }
};
```")
            }));

        // ═══════════════════════════════════════════════════════════════════
        // BACKEND COURSES
        // ═══════════════════════════════════════════════════════════════════

        // 5. Node.js & Express
        AddIfMissing(CreateCourse("Node.js & Express Cơ Bản", "nodejs-express-co-ban", "Backend", CourseLevel.Beginner, 99000,
            "Xây dựng server-side applications với Node.js và Express framework. Bao gồm REST API, middleware, authentication.",
            new[]
            {
                ("Introduction to Node.js", "Tổng quan về Node.js runtime và npm.", 15, true,
@"# Introduction to Node.js

Node.js là runtime environment chạy JavaScript phía server, xây dựng trên Chrome V8 engine.

## Khởi tạo dự án

```bash
mkdir my-api && cd my-api
npm init -y
npm install express
```

## Hello World Server

```javascript
const express = require('express');
const app = express();
const PORT = 3000;

app.get('/', (req, res) => {
    res.json({ message: 'Xin chào từ Node.js!', timestamp: new Date() });
});

app.listen(PORT, () => {
    console.log(`Server chạy tại http://localhost:${PORT}`);
});
```

## Node.js Module System

```javascript
// utils.js
const formatDate = (date) => date.toISOString().split('T')[0];
module.exports = { formatDate };

// app.js
const { formatDate } = require('./utils');
console.log(formatDate(new Date())); // 2026-08-05
```"),

                ("Express Routing & Controllers", "Thiết kế routes và controllers trong Express.", 20, false,
@"# Express Routing

## Route Organization

```javascript
// routes/courseRoutes.js
const router = require('express').Router();
const courseController = require('../controllers/courseController');

router.get('/', courseController.getAll);
router.get('/:id', courseController.getById);
router.post('/', courseController.create);
router.put('/:id', courseController.update);
router.delete('/:id', courseController.delete);

module.exports = router;
```

## Controller

```javascript
// controllers/courseController.js
const Course = require('../models/Course');

exports.getAll = async (req, res) => {
    try {
        const { page = 1, limit = 10, category } = req.query;
        const filter = category ? { category } : {};
        
        const courses = await Course.find(filter)
            .skip((page - 1) * limit)
            .limit(Number(limit))
            .sort({ createdAt: -1 });
        
        const total = await Course.countDocuments(filter);
        
        res.json({ items: courses, total, page: Number(page) });
    } catch (err) {
        res.status(500).json({ error: err.message });
    }
};
```"),

                ("Middleware in Express", "Tìm hiểu và tạo custom middleware.", 18, false,
@"# Express Middleware

Middleware là hàm có quyền truy cập vào request, response và next function.

```javascript
// Logger middleware
const logger = (req, res, next) => {
    console.log(`${req.method} ${req.url} - ${new Date().toISOString()}`);
    next();
};

// Auth middleware
const authenticate = (req, res, next) => {
    const token = req.headers.authorization?.split(' ')[1];
    if (!token) return res.status(401).json({ error: 'Token required' });
    
    try {
        const decoded = jwt.verify(token, process.env.JWT_SECRET);
        req.user = decoded;
        next();
    } catch (err) {
        res.status(401).json({ error: 'Invalid token' });
    }
};

// Sử dụng
app.use(logger);
app.use('/api/admin', authenticate, adminRoutes);
```"),

                ("Request Validation & Error Handling", "Validate dữ liệu đầu vào và xử lý lỗi.", 20, false,
@"# Validation & Error Handling

## Validation với express-validator

```javascript
const { body, validationResult } = require('express-validator');

const validateCourse = [
    body('title').trim().notEmpty().withMessage('Title is required'),
    body('price').isFloat({ min: 0 }).withMessage('Price must be >= 0'),
    body('category').isIn(['Frontend', 'Backend', 'Database']),
];

const handleValidation = (req, res, next) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    next();
};

router.post('/', validateCourse, handleValidation, courseController.create);
```

## Global Error Handler

```javascript
app.use((err, req, res, next) => {
    console.error(err.stack);
    res.status(err.status || 500).json({
        success: false,
        message: err.message || 'Internal Server Error'
    });
});
```"),

                ("JWT Authentication", "Xác thực người dùng với JSON Web Token.", 25, false,
@"# JWT Authentication

```javascript
const jwt = require('jsonwebtoken');
const bcrypt = require('bcrypt');

// Register
exports.register = async (req, res) => {
    const { email, password, fullName } = req.body;
    const hash = await bcrypt.hash(password, 10);
    const user = await User.create({ email, passwordHash: hash, fullName });
    
    const token = jwt.sign(
        { userId: user.id, role: user.role },
        process.env.JWT_SECRET,
        { expiresIn: '7d' }
    );
    
    res.status(201).json({ token, user: { id: user.id, email, fullName } });
};

// Login
exports.login = async (req, res) => {
    const { email, password } = req.body;
    const user = await User.findOne({ email });
    
    if (!user || !(await bcrypt.compare(password, user.passwordHash))) {
        return res.status(401).json({ error: 'Email hoặc mật khẩu không đúng' });
    }
    
    const token = jwt.sign(
        { userId: user.id, role: user.role },
        process.env.JWT_SECRET,
        { expiresIn: '7d' }
    );
    
    res.json({ token, user: { id: user.id, email, fullName: user.fullName } });
};
```"),

                ("File Upload & Static Files", "Upload file và phục vụ static files.", 18, false,
@"# File Upload

## Multer Setup

```javascript
const multer = require('multer');
const path = require('path');

const storage = multer.diskStorage({
    destination: './uploads/',
    filename: (req, file, cb) => {
        const uniqueName = `${Date.now()}-${Math.round(Math.random() * 1E9)}`;
        cb(null, `${uniqueName}${path.extname(file.originalname)}`);
    }
});

const upload = multer({
    storage,
    limits: { fileSize: 5 * 1024 * 1024 }, // 5MB
    fileFilter: (req, file, cb) => {
        const allowed = ['.jpg', '.jpeg', '.png', '.webp'];
        const ext = path.extname(file.originalname).toLowerCase();
        cb(null, allowed.includes(ext));
    }
});

// Route
router.post('/avatar', authenticate, upload.single('avatar'), async (req, res) => {
    const url = `/uploads/${req.file.filename}`;
    await User.updateOne({ _id: req.user.userId }, { avatarUrl: url });
    res.json({ avatarUrl: url });
});

// Serve static
app.use('/uploads', express.static('uploads'));
```"),

                ("Environment & Deployment", "Cấu hình biến môi trường và deploy ứng dụng.", 15, false,
@"# Environment & Deployment

## dotenv

```javascript
require('dotenv').config();

const config = {
    port: process.env.PORT || 3000,
    dbUri: process.env.DATABASE_URL,
    jwtSecret: process.env.JWT_SECRET,
    nodeEnv: process.env.NODE_ENV || 'development'
};

module.exports = config;
```

## Production Checklist

```javascript
// Helmet for security headers
const helmet = require('helmet');
app.use(helmet());

// Rate limiting
const rateLimit = require('express-rate-limit');
app.use('/api/', rateLimit({
    windowMs: 15 * 60 * 1000, // 15 phút
    max: 100 // Tối đa 100 request/IP
}));

// CORS
const cors = require('cors');
app.use(cors({
    origin: process.env.FRONTEND_URL,
    credentials: true
}));

// Compression
const compression = require('compression');
app.use(compression());
```")
            }));

        // 6. RESTful API Design
        AddIfMissing(CreateCourse("RESTful API Design Best Practices", "restful-api-design", "Backend", CourseLevel.Intermediate, 149000,
            "Thiết kế RESTful API chuẩn chỉnh với naming conventions, versioning, pagination, error handling.",
            new[]
            {
                ("REST Principles & URI Design", "Nguyên tắc REST và cách đặt tên URI chuẩn.", 20, true,
@"# REST API Design Principles

## URI Naming Conventions

```
# Tốt ✅
GET    /api/courses              # Lấy danh sách
GET    /api/courses/15           # Lấy chi tiết
POST   /api/courses              # Tạo mới
PUT    /api/courses/15           # Cập nhật toàn bộ
PATCH  /api/courses/15           # Cập nhật một phần
DELETE /api/courses/15           # Xóa

# Quan hệ nested
GET    /api/courses/15/lessons   # Lessons của course 15
POST   /api/courses/15/lessons   # Thêm lesson vào course 15

# Tránh ❌
GET    /api/getCourses
POST   /api/createCourse
GET    /api/course/delete/15
```

## HTTP Status Codes

| Code | Ý nghĩa |
|------|---------|
| 200  | OK - Thành công |
| 201  | Created - Tạo thành công |
| 204  | No Content - Xóa thành công |
| 400  | Bad Request - Dữ liệu không hợp lệ |
| 401  | Unauthorized - Chưa xác thực |
| 403  | Forbidden - Không có quyền |
| 404  | Not Found - Không tìm thấy |
| 500  | Internal Server Error |"),

                ("Pagination, Filtering & Sorting", "Phân trang, lọc và sắp xếp dữ liệu API.", 22, false,
@"# Pagination & Filtering

## Query Parameters

```
GET /api/courses?page=1&pageSize=10&category=Frontend&level=Beginner&sortBy=createdAt&sortDesc=true&search=react
```

## Response Format

```json
{
    ""success"": true,
    ""data"": {
        ""items"": [...],
        ""totalCount"": 45,
        ""page"": 1,
        ""pageSize"": 10,
        ""totalPages"": 5,
        ""hasNext"": true,
        ""hasPrevious"": false
    }
}
```

## Implementation

```csharp
public async Task<PagedResponse<CourseResponse>> GetAllAsync(CourseQueryParams query)
{
    query.Page = Math.Max(1, query.Page);
    query.PageSize = Math.Clamp(query.PageSize, 1, 100);

    var (items, total) = await _repository.SearchAsync(
        query.Search, query.Category, query.Level,
        query.Page, query.PageSize, query.SortBy, query.SortDesc);

    return PagedResponse<CourseResponse>.SuccessResult(
        items.Select(MapToResponse), total, query.Page, query.PageSize);
}
```"),

                ("Error Handling & Validation", "Xử lý lỗi chuẩn và validate dữ liệu API.", 18, false,
@"# API Error Handling

## Consistent Error Response

```json
{
    ""success"": false,
    ""message"": ""Validation failed."",
    ""errors"": [
        { ""field"": ""email"", ""message"": ""Email is required"" },
        { ""field"": ""password"", ""message"": ""Password must be at least 6 characters"" }
    ]
}
```

## Global Exception Middleware (C#)

```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new {
                success = false, message = ex.Message
            });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new {
                success = false, message = ""An unexpected error occurred.""
            });
        }
    }
}
```"),

                ("API Versioning", "Quản lý phiên bản API để đảm bảo backward compatibility.", 15, false,
@"# API Versioning

## Strategies

### URL Path Versioning (Phổ biến nhất)
```
GET /api/v1/courses
GET /api/v2/courses
```

### Header Versioning
```
GET /api/courses
Api-Version: 2
```

## Implementation in ASP.NET Core

```csharp
// Program.cs
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Controller
[ApiVersion(""1.0"")]
[Route(""api/v{version:apiVersion}/courses"")]
public class CoursesV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(new { version = ""v1"", data = courses });
}

[ApiVersion(""2.0"")]
[Route(""api/v{version:apiVersion}/courses"")]
public class CoursesV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(new { version = ""v2"", data = coursesWithMeta });
}
```"),

                ("Authentication & Authorization", "Bảo mật API với JWT và role-based access.", 22, false,
@"# API Security

## JWT Authentication Flow

```
1. Client POST /api/auth/login { email, password }
2. Server validates → returns { token, refreshToken }
3. Client stores token
4. Client sends: Authorization: Bearer <token>
5. Server validates token on each request
```

## Role-Based Authorization

```csharp
[ApiController]
[Route(""api/[controller]"")]
public class AdminController : ControllerBase
{
    [HttpGet(""dashboard"")]
    [Authorize(Roles = ""Admin"")]
    public IActionResult GetDashboard() { /* Admin only */ }
    
    [HttpGet(""courses"")]
    [Authorize(Roles = ""Admin,Instructor"")]
    public IActionResult GetCourses() { /* Admin or Instructor */ }
    
    [HttpGet(""profile"")]
    [Authorize] // Any authenticated user
    public IActionResult GetProfile() { /* ... */ }
}
```"),

                ("API Documentation with Swagger", "Tài liệu hóa API với Swagger/OpenAPI.", 18, false,
@"# Swagger / OpenAPI

## Setup in ASP.NET Core

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(""v1"", new OpenApiInfo
    {
        Title = ""ScrollTutor API"",
        Version = ""v1"",
        Description = ""RESTful API for e-learning platform""
    });
    
    // JWT Authentication in Swagger
    options.AddSecurityDefinition(""Bearer"", new OpenApiSecurityScheme
    {
        Name = ""Authorization"",
        Type = SecuritySchemeType.Http,
        Scheme = ""bearer"",
        BearerFormat = ""JWT""
    });
});
```

## XML Comments for Documentation

```csharp
/// <summary>Get a paginated list of courses.</summary>
/// <param name=""query"">Pagination and filter parameters.</param>
/// <response code=""200"">Returns the list of courses.</response>
[HttpGet]
[ProducesResponseType(typeof(PagedResponse<CourseResponse>), 200)]
public async Task<IActionResult> GetAll([FromQuery] CourseQueryParams query)
{
    var result = await _courseService.GetAllAsync(query);
    return Ok(result);
}
```")
            }));

        // 7. C# OOP
        AddIfMissing(CreateCourse("C# Lập Trình Hướng Đối Tượng", "csharp-oop", "Backend", CourseLevel.Beginner, 0,
            "Nắm vững 4 trụ cột OOP trong C#: Encapsulation, Inheritance, Polymorphism, Abstraction. Miễn phí hoàn toàn.",
            new[]
            {
                ("Classes, Objects & Constructors", "Lớp, đối tượng và hàm khởi tạo trong C#.", 18, true,
@"# Classes & Objects

```csharp
public class Course
{
    // Properties
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    
    // Constructor
    public Course(string title, decimal price)
    {
        Title = title;
        Price = price;
    }
    
    // Method
    public string GetPriceDisplay()
    {
        return Price == 0 ? ""Miễn phí"" : $""{Price:N0}đ"";
    }
}

// Sử dụng
var course = new Course(""C# Basics"", 99000);
Console.WriteLine(course.GetPriceDisplay()); // 99,000đ
```"),

                ("Encapsulation & Access Modifiers", "Đóng gói dữ liệu và các access modifier.", 15, false,
@"# Encapsulation

```csharp
public class BankAccount
{
    private decimal _balance; // private field
    public string Owner { get; private set; } // read-only from outside
    
    public BankAccount(string owner, decimal initialBalance)
    {
        Owner = owner;
        _balance = initialBalance;
    }
    
    public decimal Balance => _balance; // read-only property
    
    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException(""Amount must be positive"");
        _balance += amount;
    }
    
    public bool Withdraw(decimal amount)
    {
        if (amount > _balance) return false;
        _balance -= amount;
        return true;
    }
}
```

Access Modifiers: `public`, `private`, `protected`, `internal`, `protected internal`."),

                ("Inheritance & Base Classes", "Kế thừa và lớp cơ sở trong C#.", 20, false,
@"# Inheritance

```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = ""Student"";
}

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = ""Frontend"";
    public decimal Price { get; set; }
    
    public override string ToString()
        => $""[{Category}] {Title} - {Price:N0}đ"";
}

// Sử dụng
var course = new Course { Title = ""React"", Price = 199000 };
Console.WriteLine(course.Id);        // Kế thừa từ BaseEntity
Console.WriteLine(course.CreatedAt); // Kế thừa từ BaseEntity
```"),

                ("Polymorphism & Virtual Methods", "Đa hình với virtual, override, abstract.", 20, false,
@"# Polymorphism

```csharp
public abstract class Notification
{
    public string Message { get; set; }
    public abstract Task SendAsync(); // Must be implemented by subclass
}

public class EmailNotification : Notification
{
    public string ToEmail { get; set; }
    
    public override async Task SendAsync()
    {
        await EmailService.SendAsync(ToEmail, Message);
        Console.WriteLine($""Email sent to {ToEmail}"");
    }
}

public class PushNotification : Notification
{
    public string DeviceToken { get; set; }
    
    public override async Task SendAsync()
    {
        await PushService.SendAsync(DeviceToken, Message);
        Console.WriteLine($""Push sent to device"");
    }
}

// Polymorphism in action
List<Notification> notifications = new()
{
    new EmailNotification { ToEmail = ""user@mail.com"", Message = ""Chào bạn!"" },
    new PushNotification { DeviceToken = ""abc123"", Message = ""Khóa học mới!"" }
};

foreach (var n in notifications)
    await n.SendAsync(); // Gọi đúng implementation
```"),

                ("Interfaces & Dependency Injection", "Interface và tiêm phụ thuộc trong C#.", 22, false,
@"# Interfaces & DI

```csharp
// Interface
public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(int id);
    Task<IEnumerable<Course>> GetAllAsync();
    Task<Course> AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(int id);
}

// Implementation
public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;
    
    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Course?> GetByIdAsync(int id)
        => await _context.Courses.FindAsync(id);
        
    // ... other methods
}

// Register in DI (Program.cs)
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();
```

DI giúp code testable, loosely coupled và dễ thay đổi implementation."),

                ("Generics", "Lập trình tổng quát với Generics.", 18, false,
@"# Generics

```csharp
// Generic class
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    
    public static ApiResponse<T> SuccessResult(T data, string msg = """")
        => new() { Success = true, Data = data, Message = msg };
    
    public static ApiResponse<T> FailResult(string msg)
        => new() { Success = false, Message = msg };
}

// Generic repository
public class BaseRepository<T> where T : class
{
    protected readonly DbSet<T> _dbSet;
    
    public virtual async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);
    
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}

// Sử dụng
var result = ApiResponse<Course>.SuccessResult(course, ""Created!"");
```"),

                ("LINQ & Collections", "Truy vấn dữ liệu với LINQ.", 20, false,
@"# LINQ

```csharp
var students = new List<Student>
{
    new(""An"", 85, ""Frontend""),
    new(""Bình"", 62, ""Backend""),
    new(""Chi"", 92, ""Frontend""),
    new(""Dũng"", 78, ""Database"")
};

// Query syntax
var topStudents = from s in students
                  where s.Score >= 80
                  orderby s.Score descending
                  select new { s.Name, s.Score };

// Method syntax (phổ biến hơn)
var frontendAvg = students
    .Where(s => s.Category == ""Frontend"")
    .Average(s => s.Score); // 88.5

var grouped = students
    .GroupBy(s => s.Category)
    .Select(g => new {
        Category = g.Key,
        Count = g.Count(),
        AvgScore = g.Average(s => s.Score)
    });

// LINQ with EF Core
var courses = await _context.Courses
    .Where(c => c.IsPublished && c.Category == ""Backend"")
    .OrderByDescending(c => c.CreatedAt)
    .Take(10)
    .ToListAsync();
```"),

                ("Exception Handling & Best Practices", "Xử lý ngoại lệ và best practices trong C#.", 15, false,
@"# Exception Handling

```csharp
public async Task<ApiResponse<bool>> EnrollAsync(int userId, int courseId)
{
    try
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new NotFoundException($""User {userId} not found"");
        
        var course = await _courseRepo.GetByIdAsync(courseId)
            ?? throw new NotFoundException($""Course {courseId} not found"");
        
        if (await _enrollRepo.ExistsAsync(userId, courseId))
            throw new BusinessException(""Bạn đã đăng ký khóa học này rồi."");
        
        var enrollment = new Enrollment { UserId = userId, CourseId = courseId };
        await _enrollRepo.AddAsync(enrollment);
        
        return ApiResponse<bool>.SuccessResult(true, ""Đăng ký thành công!"");
    }
    catch (NotFoundException ex)
    {
        return ApiResponse<bool>.FailResult(ex.Message);
    }
    catch (BusinessException ex)
    {
        return ApiResponse<bool>.FailResult(ex.Message);
    }
}
```

## Best Practices
- Không catch Exception chung chung
- Tạo custom exceptions cho business logic
- Luôn log exception ở middleware level
- Sử dụng `throw` thay vì `throw ex` để giữ stack trace")
            }));

        // 8. Microservices .NET 9
        AddIfMissing(CreateCourse("Microservices với .NET 9", "microservices-dotnet-9", "Backend", CourseLevel.Advanced, 349000,
            "Kiến trúc Microservices thực chiến với .NET 9, Docker, RabbitMQ và API Gateway. Dành cho developer có kinh nghiệm.",
            new[]
            {
                ("Microservices Architecture Overview", "Tổng quan kiến trúc Microservices vs Monolith.", 20, true,
@"# Microservices Architecture

## Monolith vs Microservices

| Tiêu chí | Monolith | Microservices |
|----------|----------|---------------|
| Deploy | Toàn bộ ứng dụng | Từng service độc lập |
| Scaling | Scale toàn bộ | Scale từng service |
| Tech Stack | Một ngôn ngữ | Đa ngôn ngữ |
| Database | Shared DB | Database per service |
| Complexity | Đơn giản ban đầu | Phức tạp hơn |

## Khi nào dùng Microservices?
- Team lớn (5+ developers)
- Yêu cầu scale từng phần khác nhau
- Cần deploy độc lập
- Domain phức tạp, rõ ràng bounded contexts"),

                ("Building Individual Services", "Xây dựng từng microservice độc lập.", 25, false,
@"# Building a Microservice

```csharp
// CourseService/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(""CourseDb"")));

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
```

```csharp
// CourseService/Controllers/CoursesController.cs
[ApiController]
[Route(""api/[controller]"")]
public class CoursesController : ControllerBase
{
    private readonly ICourseRepository _repo;
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var courses = await _repo.GetAllAsync();
        return Ok(courses);
    }
}
```

Mỗi service có database riêng, API riêng, có thể deploy và scale độc lập."),

                ("API Gateway with Ocelot", "Thiết lập API Gateway để route requests.", 22, false,
@"# API Gateway

```json
// ocelot.json
{
    ""Routes"": [
        {
            ""DownstreamPathTemplate"": ""/api/courses/{everything}"",
            ""DownstreamScheme"": ""http"",
            ""DownstreamHostAndPorts"": [
                { ""Host"": ""course-service"", ""Port"": 5001 }
            ],
            ""UpstreamPathTemplate"": ""/api/courses/{everything}"",
            ""UpstreamHttpMethod"": [""GET"", ""POST"", ""PUT"", ""DELETE""]
        },
        {
            ""DownstreamPathTemplate"": ""/api/users/{everything}"",
            ""DownstreamScheme"": ""http"",
            ""DownstreamHostAndPorts"": [
                { ""Host"": ""user-service"", ""Port"": 5002 }
            ],
            ""UpstreamPathTemplate"": ""/api/users/{everything}""
        }
    ]
}
```

Client chỉ cần giao tiếp với 1 endpoint duy nhất (Gateway), Gateway sẽ route đến đúng service."),

                ("Inter-Service Communication", "Giao tiếp giữa các service: HTTP, gRPC, Message Queue.", 25, false,
@"# Service Communication

## Synchronous: HTTP Client

```csharp
// EnrollmentService gọi CourseService
public class CourseHttpClient
{
    private readonly HttpClient _client;
    
    public CourseHttpClient(HttpClient client)
    {
        _client = client;
        _client.BaseAddress = new Uri(""http://course-service:5001"");
    }
    
    public async Task<Course?> GetByIdAsync(int id)
    {
        var response = await _client.GetAsync($""/api/courses/{id}"");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<Course>();
    }
}
```

## Asynchronous: RabbitMQ

```csharp
// Publisher
public class EventPublisher
{
    public void PublishCourseCreated(Course course)
    {
        var message = JsonSerializer.Serialize(new {
            EventType = ""CourseCreated"",
            Data = course,
            Timestamp = DateTime.UtcNow
        });
        
        channel.BasicPublish(
            exchange: ""courses"",
            routingKey: ""course.created"",
            body: Encoding.UTF8.GetBytes(message));
    }
}
```"),

                ("Docker & Container Orchestration", "Containerize services với Docker.", 20, false,
@"# Docker for Microservices

## Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY [""CourseService.csproj"", "".""]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [""dotnet"", ""CourseService.dll""]
```

## Docker Compose

```yaml
version: '3.8'
services:
  api-gateway:
    build: ./ApiGateway
    ports: ['5000:5000']
    depends_on: [course-service, user-service]
    
  course-service:
    build: ./CourseService
    environment:
      - ConnectionStrings__CourseDb=Server=course-db;Database=Courses;...
    depends_on: [course-db]
    
  user-service:
    build: ./UserService
    depends_on: [user-db]
    
  course-db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong!Pass
```"),

                ("Health Checks & Monitoring", "Giám sát health của các services.", 18, false,
@"# Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString, name: ""database"")
    .AddRabbitMQ(rabbitConnectionString, name: ""rabbitmq"")
    .AddUrlGroup(new Uri(""http://course-service:5001/health""), name: ""course-service"");

app.MapHealthChecks(""/health"", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = ""application/json"";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});
```

## Response
```json
{
    ""status"": ""Healthy"",
    ""checks"": [
        { ""name"": ""database"", ""status"": ""Healthy"", ""duration"": 12.5 },
        { ""name"": ""rabbitmq"", ""status"": ""Healthy"", ""duration"": 3.2 }
    ]
}
```"),

                ("Distributed Logging & Tracing", "Logging tập trung và distributed tracing.", 18, false,
@"# Centralized Logging

## Serilog Setup

```csharp
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.Seq(""http://seq:5341"") // Centralized log server
        .Enrich.WithProperty(""ServiceName"", ""CourseService"")
        .Enrich.WithCorrelationId();
});
```

## Structured Logging

```csharp
public async Task<Course> CreateAsync(CreateCourseRequest request)
{
    _logger.LogInformation(
        ""Creating course {Title} in category {Category}"",
        request.Title, request.Category);
    
    var course = await _repo.AddAsync(MapToEntity(request));
    
    _logger.LogInformation(
        ""Course created successfully with ID {CourseId}"",
        course.Id);
    
    return course;
}
```

Trong microservices, centralized logging (Seq, ELK Stack) là bắt buộc để debug cross-service issues.")
            }));

        // ═══════════════════════════════════════════════════════════════════
        // DATABASE COURSES
        // ═══════════════════════════════════════════════════════════════════

        // 9. SQL Server
        AddIfMissing(CreateCourse("SQL Server Từ Cơ Bản Đến Nâng Cao", "sql-server-co-ban-nang-cao", "Database", CourseLevel.Beginner, 0,
            "Học SQL Server từ câu lệnh SELECT đến stored procedures, indexing và optimization. Miễn phí cho người mới.",
            new[]
            {
                ("Introduction to SQL & Databases", "Giới thiệu cơ sở dữ liệu quan hệ và SQL.", 12, true,
@"# SQL & Relational Databases

## Tạo Database và Table

```sql
CREATE DATABASE ScrollTutorDB;
GO

USE ScrollTutorDB;
GO

CREATE TABLE Courses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(300) NOT NULL,
    Category NVARCHAR(100),
    Price DECIMAL(18,2) DEFAULT 0,
    IsPublished BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);

CREATE TABLE Lessons (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    Title NVARCHAR(300) NOT NULL,
    OrderIndex INT DEFAULT 1,
    DurationMinutes INT DEFAULT 15,
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);
```"),

                ("SELECT Queries & Filtering", "Truy vấn dữ liệu với SELECT, WHERE, ORDER BY.", 18, false,
@"# SELECT Queries

```sql
-- Lấy tất cả khóa học
SELECT * FROM Courses;

-- Lọc và sắp xếp
SELECT Title, Category, Price
FROM Courses
WHERE IsPublished = 1 AND Category = N'Frontend'
ORDER BY Price DESC;

-- Tìm kiếm
SELECT * FROM Courses
WHERE Title LIKE N'%React%' OR Title LIKE N'%JavaScript%';

-- TOP N
SELECT TOP 5 Title, Price
FROM Courses
ORDER BY CreatedAt DESC;

-- DISTINCT
SELECT DISTINCT Category FROM Courses;

-- Aggregate functions
SELECT 
    Category,
    COUNT(*) AS TotalCourses,
    AVG(Price) AS AvgPrice,
    MAX(Price) AS MaxPrice
FROM Courses
GROUP BY Category
HAVING COUNT(*) >= 2;
```"),

                ("JOIN Operations", "Kết hợp dữ liệu từ nhiều bảng với JOIN.", 20, false,
@"# SQL JOINs

```sql
-- INNER JOIN: Chỉ lấy records có match ở cả 2 bảng
SELECT c.Title, l.Title AS LessonTitle, l.DurationMinutes
FROM Courses c
INNER JOIN Lessons l ON c.Id = l.CourseId
ORDER BY c.Title, l.OrderIndex;

-- LEFT JOIN: Lấy tất cả courses, kể cả không có lessons
SELECT c.Title, COUNT(l.Id) AS LessonCount
FROM Courses c
LEFT JOIN Lessons l ON c.Id = l.CourseId
GROUP BY c.Title;

-- Multi-table JOIN
SELECT 
    u.FullName,
    c.Title AS CourseName,
    e.ProgressPercentage,
    e.EnrolledAt
FROM Users u
INNER JOIN Enrollments e ON u.Id = e.UserId
INNER JOIN Courses c ON e.CourseId = c.Id
WHERE e.ProgressPercentage > 50
ORDER BY e.ProgressPercentage DESC;
```"),

                ("INSERT, UPDATE & DELETE", "Thao tác thêm, sửa, xóa dữ liệu.", 15, false,
@"# Data Manipulation

```sql
-- INSERT
INSERT INTO Courses (Title, Category, Price, IsPublished)
VALUES 
    (N'Vue.js 3 Mastery', N'Frontend', 149000, 1),
    (N'Python Django', N'Backend', 199000, 1);

-- UPDATE
UPDATE Courses 
SET Price = 129000, UpdatedAt = GETUTCDATE()
WHERE Id = 5;

-- UPDATE với điều kiện từ JOIN
UPDATE e
SET e.ProgressPercentage = 100, e.CompletedAt = GETUTCDATE()
FROM Enrollments e
INNER JOIN Users u ON e.UserId = u.Id
WHERE u.Role = 'Admin';

-- DELETE
DELETE FROM Courses WHERE IsPublished = 0;

-- MERGE (Upsert)
MERGE INTO Courses AS target
USING (SELECT 'react-basics' AS Slug) AS source
ON target.Slug = source.Slug
WHEN MATCHED THEN UPDATE SET UpdatedAt = GETUTCDATE()
WHEN NOT MATCHED THEN INSERT (Title, Slug) VALUES ('React Basics', 'react-basics');
```"),

                ("Subqueries & CTEs", "Truy vấn lồng nhau và Common Table Expressions.", 22, false,
@"# Subqueries & CTEs

```sql
-- Subquery trong WHERE
SELECT Title, Price
FROM Courses
WHERE Price > (SELECT AVG(Price) FROM Courses);

-- Subquery trong FROM
SELECT Category, AvgPrice
FROM (
    SELECT Category, AVG(Price) AS AvgPrice
    FROM Courses
    GROUP BY Category
) AS CategoryStats
WHERE AvgPrice > 100000;

-- CTE (Common Table Expression)
WITH CourseStats AS (
    SELECT 
        c.Id,
        c.Title,
        c.Category,
        COUNT(e.Id) AS EnrollmentCount,
        COALESCE(SUM(t.Amount), 0) AS TotalRevenue
    FROM Courses c
    LEFT JOIN Enrollments e ON c.Id = e.CourseId
    LEFT JOIN Transactions t ON c.Id = t.CourseId AND t.Status = 'SUCCESS'
    GROUP BY c.Id, c.Title, c.Category
)
SELECT * FROM CourseStats
WHERE EnrollmentCount > 0
ORDER BY TotalRevenue DESC;
```"),

                ("Indexes & Query Optimization", "Tối ưu truy vấn với indexes.", 25, false,
@"# Indexing

```sql
-- Clustered Index (tự động tạo với PRIMARY KEY)
-- Non-clustered Index
CREATE NONCLUSTERED INDEX IX_Courses_Category
ON Courses (Category)
INCLUDE (Title, Price);

-- Composite Index
CREATE INDEX IX_Courses_Category_Level
ON Courses (Category, Level)
WHERE IsPublished = 1; -- Filtered index

-- Xem execution plan
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

SELECT * FROM Courses WHERE Category = 'Frontend';
-- Kiểm tra: Logical reads, Scan count, CPU time

-- Kiểm tra index đang dùng
SELECT 
    i.name AS IndexName,
    s.user_seeks, s.user_scans, s.user_lookups,
    s.last_user_seek, s.last_user_scan
FROM sys.dm_db_index_usage_stats s
INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
WHERE s.database_id = DB_ID();
```"),

                ("Stored Procedures", "Tạo và sử dụng stored procedures.", 20, false,
@"# Stored Procedures

```sql
CREATE PROCEDURE sp_GetCoursesByCategory
    @Category NVARCHAR(100),
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@Page - 1) * @PageSize;
    
    SELECT 
        c.Id, c.Title, c.Category, c.Price, c.Level,
        COUNT(e.Id) AS EnrollmentCount,
        (SELECT COUNT(*) FROM Lessons WHERE CourseId = c.Id) AS LessonCount
    FROM Courses c
    LEFT JOIN Enrollments e ON c.Id = e.CourseId
    WHERE c.IsPublished = 1
        AND (@Category IS NULL OR c.Category = @Category)
    GROUP BY c.Id, c.Title, c.Category, c.Price, c.Level
    ORDER BY c.CreatedAt DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    
    -- Return total count
    SELECT COUNT(*) AS TotalCount
    FROM Courses
    WHERE IsPublished = 1
        AND (@Category IS NULL OR Category = @Category);
END;

-- Execute
EXEC sp_GetCoursesByCategory @Category = N'Backend', @Page = 1, @PageSize = 5;
```"),

                ("Transactions & Error Handling", "Quản lý transactions và xử lý lỗi trong SQL.", 18, false,
@"# Transactions

```sql
BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Tạo enrollment
    INSERT INTO Enrollments (UserId, CourseId, ProgressPercentage, EnrolledAt)
    VALUES (@UserId, @CourseId, 0, GETUTCDATE());
    
    -- Trừ tiền
    UPDATE Users 
    SET Balance = Balance - @CoursePrice
    WHERE Id = @UserId AND Balance >= @CoursePrice;
    
    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Số dư không đủ', 16, 1);
    END
    
    -- Ghi transaction
    INSERT INTO Transactions (UserId, CourseId, Amount, Status, PaymentTime)
    VALUES (@UserId, @CourseId, @CoursePrice, 'SUCCESS', GETUTCDATE());
    
    COMMIT TRANSACTION;
    PRINT 'Đăng ký khóa học thành công!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT ERROR_MESSAGE();
END CATCH
```

Transactions đảm bảo ACID: Atomicity, Consistency, Isolation, Durability.")
            }));

        // 10. MongoDB
        AddIfMissing(CreateCourse("MongoDB cho Developer", "mongodb-developer", "Database", CourseLevel.Intermediate, 149000,
            "Sử dụng MongoDB trong ứng dụng web: CRUD, Aggregation Pipeline, Indexing và Mongoose ODM.",
            new[]
            {
                ("MongoDB Basics & CRUD", "Thao tác CRUD cơ bản với MongoDB.", 18, true,
@"# MongoDB CRUD

```javascript
// Insert
db.courses.insertOne({
    title: 'React Basics',
    category: 'Frontend',
    price: 199000,
    level: 'Beginner',
    isPublished: true,
    tags: ['react', 'javascript', 'frontend'],
    createdAt: new Date()
});

// Find
db.courses.find({ category: 'Frontend', isPublished: true })
    .sort({ createdAt: -1 })
    .limit(10);

// Find one
db.courses.findOne({ _id: ObjectId('...') });

// Update
db.courses.updateOne(
    { _id: ObjectId('...') },
    { $set: { price: 149000 }, $inc: { enrollmentCount: 1 } }
);

// Delete
db.courses.deleteOne({ _id: ObjectId('...') });
db.courses.deleteMany({ isPublished: false });
```"),

                ("Schema Design & Relationships", "Thiết kế schema và quan hệ dữ liệu.", 22, false,
@"# MongoDB Schema Design

## Embedded Documents (1-to-few)

```javascript
// Lessons embedded trong Course
{
    _id: ObjectId('...'),
    title: 'JavaScript ES6+',
    category: 'Frontend',
    lessons: [
        { title: 'Variables', duration: 15, order: 1 },
        { title: 'Functions', duration: 20, order: 2 },
        { title: 'Async/Await', duration: 25, order: 3 }
    ]
}
```

## References (1-to-many)

```javascript
// Course
{ _id: ObjectId('course1'), title: 'React.js' }

// Enrollments - reference to course
{
    _id: ObjectId('...'),
    userId: ObjectId('user1'),
    courseId: ObjectId('course1'),  // Reference
    progress: 65,
    enrolledAt: ISODate('2026-01-15')
}

// Query with $lookup (JOIN)
db.enrollments.aggregate([
    { $lookup: {
        from: 'courses',
        localField: 'courseId',
        foreignField: '_id',
        as: 'course'
    }},
    { $unwind: '$course' }
]);
```"),

                ("Aggregation Pipeline", "Phân tích dữ liệu với Aggregation Pipeline.", 25, false,
@"# Aggregation Pipeline

```javascript
// Thống kê doanh thu theo category
db.transactions.aggregate([
    { $match: { status: 'SUCCESS' } },
    { $lookup: {
        from: 'courses',
        localField: 'courseId',
        foreignField: '_id',
        as: 'course'
    }},
    { $unwind: '$course' },
    { $group: {
        _id: '$course.category',
        totalRevenue: { $sum: '$amount' },
        totalSales: { $sum: 1 },
        avgPrice: { $avg: '$amount' }
    }},
    { $sort: { totalRevenue: -1 } },
    { $project: {
        category: '$_id',
        totalRevenue: 1,
        totalSales: 1,
        avgPrice: { $round: ['$avgPrice', 0] },
        _id: 0
    }}
]);
```

Pipeline stages chạy tuần tự, output của stage này là input của stage tiếp theo."),

                ("Mongoose ODM", "Sử dụng Mongoose trong Node.js.", 20, false,
@"# Mongoose ODM

```javascript
const mongoose = require('mongoose');

// Schema
const courseSchema = new mongoose.Schema({
    title: { type: String, required: true, trim: true, maxlength: 300 },
    slug: { type: String, unique: true, index: true },
    category: { type: String, enum: ['Frontend', 'Backend', 'Database'] },
    price: { type: Number, min: 0, default: 0 },
    level: { type: String, enum: ['Beginner', 'Intermediate', 'Advanced'] },
    isPublished: { type: Boolean, default: false },
    lessons: [{ type: mongoose.Schema.Types.ObjectId, ref: 'Lesson' }]
}, { timestamps: true });

// Pre-save hook
courseSchema.pre('save', function(next) {
    this.slug = this.title.toLowerCase().replace(/\s+/g, '-');
    next();
});

// Static method
courseSchema.statics.findByCategory = function(category) {
    return this.find({ category, isPublished: true }).sort({ createdAt: -1 });
};

const Course = mongoose.model('Course', courseSchema);
module.exports = Course;
```"),

                ("Indexing & Performance", "Tối ưu hiệu suất MongoDB với indexes.", 20, false,
@"# MongoDB Indexing

```javascript
// Single field index
db.courses.createIndex({ category: 1 });

// Compound index
db.courses.createIndex({ category: 1, level: 1, isPublished: 1 });

// Text index for search
db.courses.createIndex({ title: 'text', description: 'text' });
db.courses.find({ $text: { $search: 'javascript react' } });

// Unique index
db.courses.createIndex({ slug: 1 }, { unique: true });

// Kiểm tra index sử dụng
db.courses.find({ category: 'Frontend' }).explain('executionStats');
// Xem: totalDocsExamined vs totalKeysExamined

// Index hints
db.courses.find({ category: 'Backend' }).hint({ category: 1 });

// Drop unused index
db.courses.dropIndex('category_1');
```

**Rule of thumb**: Index các field thường xuất hiện trong WHERE, SORT, JOIN."),

                ("MongoDB Atlas & Deployment", "Deploy MongoDB lên cloud với Atlas.", 15, false,
@"# MongoDB Atlas

## Connection

```javascript
const mongoose = require('mongoose');

const connectDB = async () => {
    try {
        await mongoose.connect(process.env.MONGODB_URI, {
            dbName: 'scrolltutor',
            maxPoolSize: 10,
            serverSelectionTimeoutMS: 5000
        });
        console.log('MongoDB connected');
    } catch (err) {
        console.error('MongoDB connection error:', err.message);
        process.exit(1);
    }
};
```

## Backup & Restore

```bash
# Backup
mongodump --uri=""mongodb+srv://user:pass@cluster.mongodb.net/scrolltutor""

# Restore
mongorestore --uri=""mongodb+srv://..."" dump/scrolltutor/
```")
            }));

        // 11. Entity Framework Core
        AddIfMissing(CreateCourse("Entity Framework Core Mastery", "ef-core-mastery", "Database", CourseLevel.Intermediate, 199000,
            "Làm chủ EF Core: Migrations, LINQ queries, relationships, performance tuning và advanced patterns.",
            new[]
            {
                ("EF Core Setup & DbContext", "Cấu hình EF Core và DbContext.", 15, true,
@"# Entity Framework Core

## Setup

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

## DbContext

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<User> Users => Set<User>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }
}
```"),

                ("Fluent API & Entity Configuration", "Cấu hình entity với Fluent API.", 20, false,
@"# Entity Configuration

```csharp
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable(""Courses"");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(300);
        
        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(300);
        
        builder.HasIndex(c => c.Slug).IsUnique();
        
        builder.Property(c => c.Price)
            .HasColumnType(""decimal(18,2)"");
        
        builder.Property(c => c.Level)
            .HasConversion<string>();
        
        // Relationships
        builder.HasMany(c => c.Lessons)
            .WithOne(l => l.Course)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```"),

                ("Migrations & Database Updates", "Quản lý schema changes với Migrations.", 18, false,
@"# EF Core Migrations

```bash
# Tạo migration mới
dotnet ef migrations add AddCourseCategory

# Áp dụng migration
dotnet ef database update

# Rollback
dotnet ef database update PreviousMigrationName

# Generate SQL script
dotnet ef migrations script --idempotent -o migration.sql
```

## Migration File

```csharp
public partial class AddCourseCategory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: ""Category"",
            table: ""Courses"",
            type: ""nvarchar(100)"",
            maxLength: 100,
            nullable: true);
    }
    
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: ""Category"",
            table: ""Courses"");
    }
}
```"),

                ("LINQ Queries with EF Core", "Truy vấn dữ liệu hiệu quả với LINQ.", 22, false,
@"# LINQ Queries

```csharp
// Basic query
var frontendCourses = await _context.Courses
    .Where(c => c.Category == ""Frontend"" && c.IsPublished)
    .OrderByDescending(c => c.CreatedAt)
    .ToListAsync();

// Projection
var courseSummaries = await _context.Courses
    .Select(c => new {
        c.Id, c.Title, c.Category,
        LessonCount = c.Lessons.Count,
        EnrollmentCount = c.Enrollments.Count
    })
    .ToListAsync();

// Include (Eager Loading)
var courseWithLessons = await _context.Courses
    .Include(c => c.Lessons.OrderBy(l => l.OrderIndex))
    .FirstOrDefaultAsync(c => c.Id == courseId);

// Filtered Include
var courseActiveEnrollments = await _context.Courses
    .Include(c => c.Enrollments.Where(e => e.CompletedAt == null))
    .FirstOrDefaultAsync(c => c.Id == courseId);

// GroupBy
var statsByCategory = await _context.Courses
    .GroupBy(c => c.Category)
    .Select(g => new {
        Category = g.Key,
        Count = g.Count(),
        AvgPrice = g.Average(c => c.Price)
    })
    .ToListAsync();
```"),

                ("Relationships: 1-1, 1-Many, Many-Many", "Cấu hình các loại quan hệ trong EF Core.", 20, false,
@"# EF Core Relationships

## One-to-Many

```csharp
// Course has many Lessons
public class Course
{
    public int Id { get; set; }
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}

public class Lesson
{
    public int Id { get; set; }
    public int CourseId { get; set; }      // FK
    public Course Course { get; set; }      // Navigation
}
```

## Many-to-Many

```csharp
// User enrolls in many Courses, Course has many Users
public class Enrollment
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int CourseId { get; set; }
    public Course Course { get; set; }
    
    public decimal ProgressPercentage { get; set; }
    public DateTime EnrolledAt { get; set; }
}

// Configuration
builder.HasKey(e => new { e.UserId, e.CourseId }); // Composite key
builder.HasOne(e => e.User).WithMany(u => u.Enrollments).HasForeignKey(e => e.UserId);
builder.HasOne(e => e.Course).WithMany(c => c.Enrollments).HasForeignKey(e => e.CourseId);
```"),

                ("Repository Pattern with EF Core", "Áp dụng Repository Pattern.", 20, false,
@"# Repository Pattern

```csharp
public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public virtual async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);
    
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
}
```"),

                ("Performance: AsNoTracking, Batching, Raw SQL", "Tối ưu hiệu suất EF Core.", 22, false,
@"# EF Core Performance

## AsNoTracking (Read-only queries)

```csharp
// 30-50% faster for read-only queries
var courses = await _context.Courses
    .AsNoTracking()
    .Where(c => c.IsPublished)
    .ToListAsync();
```

## Batch Operations

```csharp
// EF Core 7+ ExecuteUpdate / ExecuteDelete
await _context.Courses
    .Where(c => c.Category == ""Frontend"")
    .ExecuteUpdateAsync(s => s
        .SetProperty(c => c.Price, c => c.Price * 0.9m)
        .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));

await _context.Lessons
    .Where(l => l.CourseId == deletedCourseId)
    .ExecuteDeleteAsync();
```

## Raw SQL

```csharp
var topCourses = await _context.Courses
    .FromSqlRaw(@""
        SELECT TOP 10 c.*
        FROM Courses c
        INNER JOIN Enrollments e ON c.Id = e.CourseId
        GROUP BY c.Id, c.Title, c.Slug, c.Description, 
                 c.Category, c.Level, c.Price, c.IsPublished,
                 c.TotalDurationMinutes, c.TotalLessons,
                 c.ThumbnailUrl, c.CreatedAt, c.UpdatedAt
        ORDER BY COUNT(e.Id) DESC"")
    .ToListAsync();
```")
            }));

        // 12. Database Performance
        AddIfMissing(CreateCourse("Database Performance & Optimization", "db-performance", "Database", CourseLevel.Advanced, 249000,
            "Kỹ thuật tối ưu database nâng cao: Query plans, indexing strategies, partitioning, caching.",
            new[]
            {
                ("Query Execution Plans", "Đọc và phân tích execution plans.", 25, true,
@"# Query Execution Plans

## Đọc Execution Plan

```sql
-- Bật actual execution plan
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

-- Query cần phân tích
SELECT c.Title, COUNT(e.Id) AS Enrollments
FROM Courses c
LEFT JOIN Enrollments e ON c.Id = e.CourseId
WHERE c.Category = 'Frontend'
GROUP BY c.Title
ORDER BY Enrollments DESC;
```

## Các loại Scan

| Operation | Chi phí | Mô tả |
|-----------|---------|-------|
| Index Seek | Thấp ✅ | Tìm trực tiếp qua index |
| Index Scan | Trung bình | Quét toàn bộ index |
| Table Scan | Cao ❌ | Quét toàn bộ bảng |
| Key Lookup | Trung bình | Tra cứu thêm data từ clustered index |

Mục tiêu: Chuyển Table Scan → Index Seek bằng cách tạo index phù hợp."),

                ("Advanced Indexing Strategies", "Chiến lược indexing nâng cao.", 30, false,
@"# Advanced Indexing

```sql
-- Covering Index (INCLUDE columns)
CREATE NONCLUSTERED INDEX IX_Courses_Category_Covering
ON Courses (Category, IsPublished)
INCLUDE (Title, Price, Level, TotalLessons);

-- Filtered Index
CREATE INDEX IX_Courses_Published_Frontend
ON Courses (Price DESC)
WHERE IsPublished = 1 AND Category = 'Frontend';

-- Columnstore Index (for analytics)
CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_Transactions
ON Transactions (Amount, PaymentTime, Status, CourseId);

-- Index maintenance
ALTER INDEX ALL ON Courses REBUILD;
ALTER INDEX IX_Courses_Category ON Courses REORGANIZE;
```

## Index Design Checklist
1. WHERE clause columns → Leading columns
2. JOIN columns → Index foreign keys
3. ORDER BY columns → Include in index
4. SELECT columns → INCLUDE to avoid key lookups
5. Filter out unused indexes regularly"),

                ("Query Optimization Techniques", "Kỹ thuật tối ưu truy vấn.", 28, false,
@"# Query Optimization

## Avoid SELECT *

```sql
-- Bad ❌
SELECT * FROM Courses WHERE Category = 'Frontend';

-- Good ✅
SELECT Id, Title, Price, Level FROM Courses WHERE Category = 'Frontend';
```

## Use EXISTS instead of IN

```sql
-- Slower with large subsets
SELECT * FROM Users u
WHERE u.Id IN (SELECT UserId FROM Enrollments);

-- Faster
SELECT * FROM Users u
WHERE EXISTS (SELECT 1 FROM Enrollments e WHERE e.UserId = u.Id);
```

## Avoid functions on indexed columns

```sql
-- Index không được sử dụng ❌
SELECT * FROM Courses WHERE YEAR(CreatedAt) = 2026;

-- Index được sử dụng ✅
SELECT * FROM Courses 
WHERE CreatedAt >= '2026-01-01' AND CreatedAt < '2027-01-01';
```

## Pagination optimization

```sql
-- Keyset pagination (faster than OFFSET for large datasets)
SELECT TOP 10 Id, Title, Price
FROM Courses
WHERE Id > @LastId
ORDER BY Id;
```"),

                ("Table Partitioning", "Phân vùng bảng lớn để cải thiện hiệu suất.", 25, false,
@"# Table Partitioning

```sql
-- Create partition function
CREATE PARTITION FUNCTION pf_TransactionDate (DATETIME2)
AS RANGE RIGHT FOR VALUES (
    '2025-01-01', '2025-07-01',
    '2026-01-01', '2026-07-01',
    '2027-01-01'
);

-- Create partition scheme
CREATE PARTITION SCHEME ps_TransactionDate
AS PARTITION pf_TransactionDate
ALL TO ([PRIMARY]);

-- Create partitioned table
CREATE TABLE TransactionsPartitioned (
    Id INT IDENTITY(1,1),
    UserId INT NOT NULL,
    Amount DECIMAL(18,2),
    PaymentTime DATETIME2 NOT NULL,
    Status NVARCHAR(50),
    CONSTRAINT PK_Trans PRIMARY KEY (Id, PaymentTime)
) ON ps_TransactionDate(PaymentTime);
```

Partitioning giúp truy vấn trên dữ liệu lớn (hàng triệu records) nhanh hơn bằng cách chỉ scan partition liên quan."),

                ("Caching Strategies", "Chiến lược caching cho database.", 22, false,
@"# Database Caching

## In-Memory Cache (C#)

```csharp
public class CachedCourseService : ICourseService
{
    private readonly ICourseService _inner;
    private readonly IMemoryCache _cache;
    
    public async Task<List<Course>> GetPopularCoursesAsync()
    {
        const string key = ""popular_courses"";
        
        if (_cache.TryGetValue(key, out List<Course>? cached))
            return cached!;
        
        var courses = await _inner.GetPopularCoursesAsync();
        
        _cache.Set(key, courses, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(3)
        });
        
        return courses;
    }
}
```

## Redis Distributed Cache

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = ""localhost:6379"";
    options.InstanceName = ""ScrollTutor_"";
});

// Usage
await _distributedCache.SetStringAsync(
    $""course:{id}"",
    JsonSerializer.Serialize(course),
    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }
);
```"),

                ("Monitoring & Troubleshooting", "Giám sát và xử lý sự cố database.", 20, false,
@"# Database Monitoring

## SQL Server DMVs

```sql
-- Top 10 slow queries
SELECT TOP 10
    qs.total_elapsed_time / qs.execution_count / 1000 AS avg_ms,
    qs.execution_count,
    SUBSTRING(qt.text, qs.statement_start_offset/2 + 1, 100) AS query_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
ORDER BY avg_ms DESC;

-- Missing indexes
SELECT 
    d.statement AS table_name,
    d.equality_columns,
    d.inequality_columns,
    d.included_columns,
    s.avg_user_impact
FROM sys.dm_db_missing_index_details d
INNER JOIN sys.dm_db_missing_index_groups g ON d.index_handle = g.index_handle
INNER JOIN sys.dm_db_missing_index_group_stats s ON g.index_group_handle = s.group_handle
ORDER BY s.avg_user_impact DESC;

-- Lock monitoring
SELECT 
    l.request_session_id,
    l.resource_type,
    l.request_mode,
    l.request_status,
    OBJECT_NAME(p.object_id) AS table_name
FROM sys.dm_tran_locks l
LEFT JOIN sys.partitions p ON l.resource_associated_entity_id = p.hobt_id;
```

Regularly review slow queries, missing indexes, và blocking sessions để giữ database healthy.")
            }));

        // Unpublish old legacy English courses (ID 1-4) to prevent duplicate content and language mismatch
        var legacySlugs = new[] { "aspnet-core-9-complete-guide", "react-18-typescript-masterclass", "sql-server-ef-core-deep-dive", "html-basic" };
        var legacyCourses = await context.Courses.Where(c => legacySlugs.Contains(c.Slug)).ToListAsync();
        foreach (var legacy in legacyCourses)
        {
            legacy.IsPublished = false;
        }

        if (courses.Count > 0)
        {
            context.Courses.AddRange(courses);
        }

        await context.SaveChangesAsync();

        // Seed 18 student users created between July 23 and July 29, 2026 with course progress
        await SeedNewUsersAndProgressAsync(context);
    }

    private static async Task SeedNewUsersAndProgressAsync(AppDbContext context)
    {
        var newUserData = new (string Email, string Name, DateTime CreatedAt)[]
        {
            ("an.nguyen237@gmail.com", "Nguyễn Văn An", new DateTime(2026, 7, 23, 8, 30, 0)),
            ("binh.tran237@gmail.com", "Trần Thị Bình", new DateTime(2026, 7, 23, 14, 15, 0)),
            ("cuong.le247@gmail.com", "Lê Hoàng Cường", new DateTime(2026, 7, 24, 9, 45, 0)),
            ("duc.pham247@gmail.com", "Phạm Minh Đức", new DateTime(2026, 7, 24, 16, 20, 0)),
            ("em.vu257@gmail.com", "Vũ Thị Em", new DateTime(2026, 7, 25, 10, 10, 0)),
            ("phong.dang257@gmail.com", "Đặng Quốc Phong", new DateTime(2026, 7, 25, 19, 05, 0)),
            ("gia.bui267@gmail.com", "Bùi Thanh Gia", new DateTime(2026, 7, 26, 11, 30, 0)),
            ("hai.do267@gmail.com", "Đỗ Như Hải", new DateTime(2026, 7, 26, 15, 40, 0)),
            ("khanh.ho277@gmail.com", "Hồ Văn Khánh", new DateTime(2026, 7, 27, 8, 15, 0)),
            ("lam.ngo277@gmail.com", "Ngô Thị Lâm", new DateTime(2026, 7, 27, 13, 50, 0)),
            ("nam.duong287@gmail.com", "Dương Tuấn Nam", new DateTime(2026, 7, 28, 9, 20, 0)),
            ("oanh.ly287@gmail.com", "Lý Thị Oanh", new DateTime(2026, 7, 28, 14, 10, 0)),
            ("phuc.phan287@gmail.com", "Phan Văn Phúc", new DateTime(2026, 7, 28, 20, 30, 0)),
            ("quynh.vo297@gmail.com", "Võ Thị Quỳnh", new DateTime(2026, 7, 29, 7, 45, 0)),
            ("tam.trinh297@gmail.com", "Trịnh Minh Tâm", new DateTime(2026, 7, 29, 10, 15, 0)),
            ("uyen.dinh297@gmail.com", "Đinh Thị Uyên", new DateTime(2026, 7, 29, 12, 40, 0)),
            ("viet.hoang297@gmail.com", "Hoàng Văn Việt", new DateTime(2026, 7, 29, 16, 50, 0)),
            ("xuan.nguyen297@gmail.com", "Nguyễn Thanh Xuân", new DateTime(2026, 7, 29, 21, 10, 0))
        };

        var freeCourses = await context.Courses
            .Include(c => c.Lessons)
            .Where(c => c.Price == 0 && c.IsPublished)
            .ToListAsync();

        if (freeCourses.Count == 0) return;

        await context.Database.ExecuteSqlRawAsync("DELETE FROM transactions WHERE transaction_id LIKE 'SEED_%'");

        var feCourse = freeCourses.FirstOrDefault(c => c.Category == "Frontend") ?? freeCourses[0];
        var beCourse = freeCourses.FirstOrDefault(c => c.Category == "Backend") ?? freeCourses[0];
        var dbCourse = freeCourses.FirstOrDefault(c => c.Category == "Database") ?? freeCourses[0];

        var defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123456");

        foreach (var userData in newUserData)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == userData.Email);
            if (existingUser == null)
            {
                existingUser = new User
                {
                    Email = userData.Email,
                    FullName = userData.Name,
                    PasswordHash = defaultPasswordHash,
                    Role = UserRole.Student,
                    IsVerified = true,
                    CreatedAt = userData.CreatedAt,
                    UpdatedAt = userData.CreatedAt
                };
                context.Users.Add(existingUser);
                await context.SaveChangesAsync();
            }
            else
            {
                existingUser.CreatedAt = userData.CreatedAt;
                context.Entry(existingUser).Property(u => u.CreatedAt).IsModified = true;
                await context.SaveChangesAsync();
            }

            var coursesToEnroll = new List<Course> { feCourse, beCourse, dbCourse };
            int userIndex = Array.IndexOf(newUserData, userData);

            foreach (var course in coursesToEnroll)
            {
                var enrollment = await context.Enrollments
                    .FirstOrDefaultAsync(e => e.UserId == existingUser.Id && e.CourseId == course.Id);

                bool isCompleted = (userIndex % 3 != 0); // ~66% completion rate

                if (enrollment == null)
                {
                    enrollment = new Enrollment
                    {
                        UserId = existingUser.Id,
                        CourseId = course.Id,
                        EnrolledAt = userData.CreatedAt.AddMinutes(15),
                        LastAccessedAt = userData.CreatedAt.AddHours(2),
                        ProgressPercentage = isCompleted ? 100 : 50,
                        CompletedAt = isCompleted ? userData.CreatedAt.AddDays(1) : null
                    };
                    context.Enrollments.Add(enrollment);
                }
                else if (isCompleted && enrollment.CompletedAt == null)
                {
                    enrollment.ProgressPercentage = 100;
                    enrollment.CompletedAt = userData.CreatedAt.AddDays(1);
                }

                await context.SaveChangesAsync();

                var lessons = course.Lessons.OrderBy(l => l.OrderIndex).ToList();
                if (lessons.Count > 0)
                {
                    int lessonsToComplete = isCompleted ? lessons.Count : lessons.Count / 2;
                    for (int i = 0; i < lessons.Count; i++)
                    {
                        var lesson = lessons[i];
                        var progress = await context.UserLessonProgresses
                            .FirstOrDefaultAsync(p => p.UserId == existingUser.Id && p.LessonId == lesson.Id);

                        bool lessonCompleted = i < lessonsToComplete;
                        if (progress == null)
                        {
                            context.UserLessonProgresses.Add(new UserLessonProgress
                            {
                                UserId = existingUser.Id,
                                LessonId = lesson.Id,
                                Completed = lessonCompleted,
                                CompletedAt = lessonCompleted ? userData.CreatedAt.AddHours(i + 1) : null,
                                WatchTimeSeconds = lessonCompleted ? lesson.DurationMinutes * 60 : 300,
                                LastPositionSeconds = lessonCompleted ? lesson.DurationMinutes * 60 : 150,
                                CreatedAt = userData.CreatedAt.AddMinutes(20),
                                UpdatedAt = userData.CreatedAt.AddHours(i + 1)
                            });
                        }
                        else if (lessonCompleted && !progress.Completed)
                        {
                            progress.Completed = true;
                            progress.CompletedAt = userData.CreatedAt.AddHours(i + 1);
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }

            // Clean up any seeded transactions so newly seeded accounts have 0 balance and 0 deposited
            var seededTxList = await context.Transactions
                .Where(t => t.UserId == existingUser.Id && t.TransactionId.StartsWith("SEED_"))
                .ToListAsync();
            if (seededTxList.Count > 0)
            {
                context.Transactions.RemoveRange(seededTxList);
                await context.SaveChangesAsync();
            }
        }
    }

    // ── Helper ──────────────────────────────────────────────────────────

    private static Course CreateCourse(
        string title, string slug, string category, CourseLevel level,
        decimal price, string description,
        (string Title, string Desc, int Duration, bool IsFree, string Content)[] lessons)
    {
        var course = new Course
        {
            Title = title,
            Slug = slug,
            Description = description,
            Category = category,
            Level = level,
            Price = price,
            IsPublished = true,
            TotalLessons = lessons.Length,
            TotalDurationMinutes = lessons.Sum(l => l.Duration),
            Lessons = lessons.Select((l, i) => new Lesson
            {
                Title = l.Title,
                Description = l.Desc,
                Content = l.Content,
                DurationMinutes = l.Duration,
                OrderIndex = i + 1,
                IsFree = l.IsFree
            }).ToList()
        };
        return course;
    }
}
