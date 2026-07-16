import { useState, useEffect } from "react"
import { useParams, useNavigate } from "react-router-dom"
import "../App.css"
import { Plant } from "../types/Plant"

function PlantDetail() {
    const { plantId } = useParams<{ plantId: string }>()
    const navigate = useNavigate()
    const [plant, setPlant] = useState<Plant | null>(null)
    const [loading, setLoading] = useState<boolean>(true)
    const [error, setError] = useState<string | null>(null)

    useEffect (() => {
        fetch(`https://localhost:7288/api/plants/id/${plantId}`)
            .then((res: Response) => {
                if (!res.ok) throw new Error("Failed to fetch plant details")
                return res.json()
            })
            .then((data: Plant) => {
                setPlant(data)
                setLoading(false)
            })
            .catch((err: Error) => {
                setError(err.message)
                setLoading(false)
            })
    }, [plantId])

    if (loading) return <div className="status">Loading plant...</div>
    if (error) return <div className="status error">Error: {error}</div>
    if (!plant) return <div className="status">Plant not found.</div>

    return (
        <div className="app">
            <button className="back-button" onClick={() => navigate("/")}>
                ← Back
            </button>
            <div className="detail-card">
                <h1>{plant.name}</h1>
                <div className="detail-rows">
                    <div className="detail-row">
                        <span className="label">Type</span>
                        <span className="value">{plant.type}</span>
                    </div>
                    <div className="detail-row">
                        <span className="label">Life Cycle</span>
                        <span className="value">{plant.lifeCycle}</span>
                    </div>
                    <div className="detail-row">
                        <span className="label">Flowering Status</span>
                        <span className="value">{plant.floweringStatus ? "Yes" : "No"}</span>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default PlantDetail
