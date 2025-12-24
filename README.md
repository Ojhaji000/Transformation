# Transformation

## 📌 Overview
This project demonstrates **translation** and **rotation** of 2D shapes in a 2D plane using matrix mathematics and homogeneous coordinates.

## 🎯 Why This Project is Useful
- Shows how to perform 2D geometric transformations step by step.
- Useful in **computer graphics**, **CAD systems**, and **rendering pipelines** where precise control over shape transformations is required.
- Provides a foundation for learning more advanced transformations (scaling, reflection, shearing).

## 🚀 Getting Started
1. Download or clone the repository.
2. Open the project in **Visual Studio**, having winforms installed from the Visual Studio Installer.
3. Press **F5** to build and run the application.

## 🧩 Code Structure
- **Main Program**: Handles user input and initializes transformations.
- **Transformation Module**: Implements matrix-based translation and rotation.
- **Utilities**: Helper functions for coordinate handling and visualization.  
../Transformation/  
├── CoordinateSystem.cs // for adjusting the coordinate system of winforms as per the convemtaional one  
├── Form1.Designer.cs  
├── Form1.cs // conatins all the calls for different transformation  
├── Form1.resx  
├── Images // for future, improving readme  
│   ├── combinedDemo.gif  
│   ├── rotationDemo.gif  
│   └── translationDemo.gif  
├── Matrix.cs  //contains matrix operations independent of transformation  
├── Program.cs  
├── README.md  
├── Transformation.csproj  
├── Transformation.csproj.user  
└── Transformation.slnx  

2 directories, 13 files

## 📐 Mathematics Behind the Project
- **Translation**:  
  Represented as matrix multiplication with homogeneous coordinates:  
  The matrix A is  
$`
    \begin{bmatrix} 1 & 0 & t \\ 0 & 1 & t \\ 0 & 0 & 1 \end{bmatrix}
    \cdot
    \begin{bmatrix}
    x \\  y \\  1
    \end{bmatrix}=
    \begin{bmatrix} x' \\ y' \\ 1\end{bmatrix}
 `$

- **Rotation**:  
  Rotation by angle θ around the origin:  
$`
\begin{bmatrix}
    \cos\theta & -\sin\theta \\
    \sin\theta & \cos\theta \\
\end{bmatrix}
\cdot
\begin{bmatrix} x \\  y \end{bmatrix}=
\begin{bmatrix} x' \\ y'\end{bmatrix}
`$

## ✨ Features
- Matrix-based **2D Translation** (x, y, 1).
- Matrix-based **2D Rotation** around the origin.
- Easy-to-extend framework for adding more transformations.

## 📚 Future Improvements
- Add scaling and reflection transformations.
- Implement GUI visualization for interactive shape manipulation.
- Extend to 3D transformations.


